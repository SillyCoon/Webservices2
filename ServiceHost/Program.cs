using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Description;

namespace Host
{
    [ServiceContract]
    public interface IStringService
    {
        [OperationContract]
        int GetWordCount(string text);
    }

    public class StringService : IStringService
    {
        private char[] delimeters = Enumerable.Range(0, 256)
            .Select(i => (char)i)
            .Where(c => char.IsWhiteSpace(c) || char.IsPunctuation(c))
            .ToArray();

        public int GetWordCount(string s)
        {
            string[] words = s.Split(delimeters,
                StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
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
