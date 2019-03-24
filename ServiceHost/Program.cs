using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Description;
using KNN = KNN.KNN;
using KNN;

namespace Host
{

    public struct PrivateInfo
    {
        public int Clients { get; set; }
        public int Requests { get; set; }
    }

    [ServiceContract]
    public interface IStringService
    {
        [OperationContract]
        Task<string> DefineClass(string unknownDataset);

        [OperationContract]
        string AddString(string knownDataset);

        [OperationContract]
        Task<DatasetInfo> GetDatasetInformation();

        [OperationContract]
        PrivateInfo GetPrivateInformation();

        [OperationContract]
        void RegisterClient();

        [OperationContract]
        void UnregisterClient();

        [OperationContract]
        void IncreaseRequestsQuantity();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class StringService : IStringService
    {
        public int ClientQuantity;
        public int RequestsQuantity;

        private global::KNN.KNN classDefiner = new global::KNN.KNN();

        public async Task<string> DefineClass(string unknownDataset)
        {
            return await classDefiner.DefineSetClass(unknownDataset);
        }

        public string AddString(string knownDataset)
        {
            classDefiner.AddString(knownDataset);
            return "Success";
        }

        public async Task<DatasetInfo> GetDatasetInformation()
        {
            var kek = await classDefiner.GetDatasetInfo();
            return kek;
        }

        public PrivateInfo GetPrivateInformation()
        {
            return new PrivateInfo {Clients = ClientQuantity, Requests = RequestsQuantity};
        }
        public void RegisterClient()
        {
            object syncObj = new object();
            lock (syncObj)
            {
                ClientQuantity++;
            }
        }

        public void UnregisterClient()
        {
            object unregister = new object();
            lock (unregister)
            {
                ClientQuantity--;
            }
        }

        public void IncreaseRequestsQuantity()
        {
            object incRequests = new object();
            lock (incRequests)
            {
                RequestsQuantity++;
            }
        }
    }
    class Program
    {
      

        static void Main(string[] args)
        {
            string uri = "http://localhost:8080/StringService";
            ServiceHost host = new ServiceHost(typeof(StringService),
                new Uri(uri));

            host.AddServiceEndpoint(typeof(IStringService),
                new BasicHttpBinding(),
                "");

            ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            behavior.HttpGetEnabled = true;
            host.Description.Behaviors.Add(behavior);
            host.AddServiceEndpoint(typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            host.Open();

            Console.WriteLine("Host was started at " + uri);

            // Waiting for interrupt
            Console.ReadKey();

            host.Close();
        }
    }
}
