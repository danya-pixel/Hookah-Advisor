using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hookah_Advisor
{
    public class TableParser
    {
        private readonly string _fileName;

        public TableParser(string fileName)
        {
            _fileName = fileName;
        }

        private Dictionary<int, Dictionary<string, List<string>>> LoadJson()
        {
            var jObject = JObject.Parse(File.ReadAllText("../../../" + _fileName));
            return jObject.ToObject<Dictionary<int, Dictionary<string, List<string>>>>();
        }

        public void WriteToJson(Dictionary<int, Tobacco> database)
        {
            var jsonDict = JsonDictFromDatabase(database);
            File.WriteAllText("../../../" + _fileName + '3', JsonConvert.SerializeObject(jsonDict));
        }

        public Dictionary<int, Tobacco> GetTobaccosFromJson()
        {
            var json = LoadJson();
            var tobaccoDatabase = new Dictionary<int, Tobacco>();
            foreach (var (id, tobaccoData) in json)
            {
                var tobacco = new Tobacco {id = id};
                foreach (var (param, value) in tobaccoData)
                {
                    switch (param)
                    {
                        case "name":
                            tobacco.name = value[0];
                            break;
                        case "brand":
                            tobacco.brand = value[0];
                            break;
                        case "categories":
                            tobacco.categories = value;
                            break;
                        case "tastes":
                            tobacco.tastes = value;
                            break;
                        case "description":
                            tobacco.description = value[0];
                            break;
                    }
                }

                tobaccoDatabase[id] = tobacco;
            }

            return tobaccoDatabase;
        }

        private static Dictionary<int, Dictionary<string, List<string>>> JsonDictFromDatabase(
            Dictionary<int, Tobacco> tobaccoDatabase)
        {
            var tobaccoJsonDict = new Dictionary<int, Dictionary<string, List<string>>>();
            foreach (var (id, tobacco) in tobaccoDatabase)
            {
                var name = new List<string>(new[] {tobacco.name});
                var brand = new List<string>(new[] {tobacco.brand});
                var description = new List<string>(new[] {tobacco.description});
                var tobaccoDict = new Dictionary<string, List<string>>()
                {
                    {"name", name},
                    {"brand", brand},
                    {"categories", tobacco.categories},
                    {"tastes", tobacco.tastes},
                    {"description", description}
                };
                tobaccoJsonDict[id] = tobaccoDict;
            }

            return tobaccoJsonDict;
        }
    }
}