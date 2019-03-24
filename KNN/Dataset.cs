using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KNN
{
    public struct DatasetInfo
    {
        public int ColumnNumber { get; set; }
        public int RowsNumber { get; set; }
        public Dictionary<string, int> ClassDistribution { get; set; }
    }
    public class KNN
    {


        string wineFileName = "../../wines.csv";

        public List<string[]> Data { get; set; }

        public int K { get; } = 7;

        public KNN()
        {

        }

        public async Task<DatasetInfo> GetDatasetInfo()
        {
            var dataSet = await GetDataFromCsvAsync();
            var datasetLength = dataSet.Count;
            var columnNumber = dataSet[0].Length;
            var classDistribution = ClassDistribution(dataSet);
            return new DatasetInfo
            {
                ColumnNumber = columnNumber,
                RowsNumber = datasetLength,
                ClassDistribution = classDistribution
            };
        }

        public int GetDatasetLength => Data.Count;

        public int ColumnsQuantity => Data[0].Length;

        // Euclid Distance, return K nearest neighbors
        private Dictionary<double, string[]> ProximityMeasure(string[] definingSet, List<string[]> dataSet)
        {
            Dictionary<double, string[]> measures = new Dictionary<double, string[]>();
            try
            {

                foreach (var set in dataSet)
                {
                    double result = 0;
                    for (int i = 1; i < dataSet[0].Length; i++)
                    {
                        double defValue = Convert.ToDouble(definingSet[i][0] == '.' ? definingSet[i].Replace(".", "0,") : definingSet[i].Replace('.', ','));
                        double setValue = Convert.ToDouble(set[i][0] == '.' ? set[i].Replace(".", "0,") : set[i].Replace('.', ','));
                        result += Math.Pow(defValue - setValue, 2);
                    }

                    measures.Add(Math.Sqrt(result), set);
                }

                var res = measures.OrderBy(item => item.Key).Take(K).ToDictionary(item => item.Key, item => item.Value);
                return res;

            } catch(Exception e)
            {
                
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }
             
        } 

        // Define class of object
        public async Task<string> DefineSetClass(string definingDataset)
        {
            List<string[]> data = await GetDataFromCsvAsync();

            var neighbors = ProximityMeasure(StringToSet(definingDataset), data);
            var temp = neighbors.GroupBy(item => item.Value[0], (item, t) => new {Key = item, Cnt = t.Count()})
                .ToList();
            var result = neighbors
                .GroupBy(item => item.Value[0], (item, t) => new {Key = item, Cnt = t.Count()})
                .OrderByDescending(item => item.Cnt).Select(item => item.Key)
                .FirstOrDefault();

            return result;
        }

        // Add new string to our dataset
        public async void AddString(string data)
        {
            using (StreamWriter stream = new StreamWriter(wineFileName, append: true))
            {
                await stream.WriteAsync(data + "\n");
            }
        }

        // How many objects belongs to classes
        public Dictionary<string, int> ClassDistribution(List<string[]> dataSet)
        {
            var result = dataSet
                .GroupBy(item => item[0])
                .Select(item => (new {key = item.Key, cnt = item.Count()}))
                .ToDictionary(item => item.key, item => item.cnt);
            return result;
        }

        // Private methods to get data from csv file
        private string[] StringToSet(string str)
        {
            var csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            return csvParser.Split(str);
        }

        public async void GetDataFromCsv()
        {
            Data = new List<string[]>();
            using (StreamReader reader = new StreamReader(wineFileName))
            {
                string line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] wine = StringToSet(line);
                    Data.Add(wine);
                }
            }
        }

        public async Task<List<string[]>> GetDataFromCsvAsync()
        {
            List<string[]> data = new List<string[]>();
            using (StreamReader reader = new StreamReader(wineFileName))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    data.Add(StringToSet(line));
                }

                return data;
            }
        }
    }
}

