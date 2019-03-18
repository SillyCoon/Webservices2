using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KNN
{
    public class KNN
    {
        string wineFileName = "../../wines.csv";

        public List<string[]> Data { get; set; }

        public int K { get; } = 7;

        public KNN()
        {
            
        }

        public int GetDatasetLength => Data.Count;

        public int ColumnsQuantity => Data[0].Length;

        // Euclid Distance, return K nearest neighbors
        public Dictionary<double, string[]> ProximityMeasure(string[] definingSet)
        {
           Dictionary<double, string[]> measures = new Dictionary<double, string[]>();

            foreach (var set in Data)
            {
                double result = 0;
                for (int i = 1; i < ColumnsQuantity; i++)
                {
                    result += Math.Pow(Convert.ToDouble(definingSet[i].Replace('.', ',')) - Convert.ToDouble(set[i].Replace('.', ',')), 2);
                }
                measures.Add(Math.Sqrt(result), set);
            }

            var res = measures.OrderBy(item => item.Key).Take(K).ToDictionary(item => item.Key, item => item.Value);
            return res;
        }

        // Define class of object
        public string DefineSetClass(string definingDataset)
        {
            GetDataFromCsv();
            
            var neighbors = ProximityMeasure(StringToSet(definingDataset));
            var temp = neighbors.GroupBy(item => item.Value[0], (item, t) => new {Key = item, Cnt = t.Count()})
                .ToList();
            var result = neighbors
                .GroupBy(item => item.Value[0], (item, t) => new {Key = item, Cnt = t.Count()})
                .OrderByDescending(item => item.Cnt).Select(item => item.Key)
                .FirstOrDefault();

            return result;
        }
        // Add new string to our dataset
        public void AddString(string data)
        {
            using (StreamWriter stream = new StreamWriter(wineFileName, append: true))
            {
                stream.Write(data + "\n");
            }
        }
        // How many objects belongs to classes
        public Dictionary<string, int> ClassDistribution()
        {
            GetDataFromCsv();

            var result = Data
                .GroupBy(item => item[0])
                .Select(item => (new {key = item.Key, cnt = item.Count()}))
                .ToDictionary(item => item.key, item=> item.cnt);
            return result;
        }

        // Private methods to get data from csv file
        private string[] StringToSet(string str)
        {
            var csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            return csvParser.Split(str);
        }

        public void GetDataFromCsv()
        {
            Data = new List<string[]>();
            using (StreamReader reader = new StreamReader(wineFileName))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] wine = StringToSet(line);
                    Data.Add(wine);
                }
            }
        }
    }
}
