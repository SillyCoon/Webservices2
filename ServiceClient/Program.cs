using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Discovery;


namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new StringServiceClient();
            int n = client.GetWordCount("Hello world!");
            Console.WriteLine(n);
            Console.ReadKey();
        }
    }
}
