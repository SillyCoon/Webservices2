using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Description;
using KNN = KNN.KNN;

namespace Host
{
    public struct DatasetInfo
    {
        public int ColumnNumber { get; set; }
        public int RowsNumber { get; set; }
        public Dictionary<string, int> ClassDistribution { get; set; }
    }

    [ServiceContract]
    public interface IStringService
    {
        [OperationContract]
        string DefineClass(string unknownDataset);

        [OperationContract]
        string AddString(string knownDataset);

        [OperationContract]
        DatasetInfo GetDatasetInformation();

        [OperationContract]
        string GetPrivateInformation();
    }

    public class StringService : IStringService
    {

        private global::KNN.KNN classDefiner = new global::KNN.KNN();

        public string DefineClass(string unknownDataset)
        {
            return classDefiner.DefineSetClass(unknownDataset);
        }

        public string AddString(string knownDataset)
        {
            classDefiner.AddString(knownDataset);
            return "Success";
        }

        public DatasetInfo GetDatasetInformation()
        {
            classDefiner.GetDataFromCsv();
            var datasetLength = classDefiner.GetDatasetLength;
            var columnNumber = classDefiner.ColumnsQuantity;
            var classDistribution = classDefiner.ClassDistribution();
            return  new DatasetInfo
            {
                ColumnNumber = columnNumber,
                RowsNumber = datasetLength,
                ClassDistribution = classDistribution
            };
        }

        public string GetPrivateInformation()
        {
            throw new NotImplementedException();
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
