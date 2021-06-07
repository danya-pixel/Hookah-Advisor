using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Hookah_Advisor.Parsers
{
    public class TobaccoParser:IParser<Tobacco>
    {
        public Dictionary<int, Tobacco> Load(string fileName)
        {
            var str = File.ReadAllText("Source/" + fileName);
            var tobaccoList = JsonConvert.DeserializeObject<List<Tobacco>>(str);

            var tobaccoDict = new Dictionary<int, Tobacco>();
            if (tobaccoList == null) return tobaccoDict;
            
            foreach (var tobacco in tobaccoList)
            {
                tobaccoDict[tobacco.Id] = tobacco;
            }

            return tobaccoDict;
        }
        public void Write(Dictionary<int, Tobacco> database,string fileName)
        {
            var tobaccoList = database.Cast<Tobacco>().ToList();
            File.WriteAllText("Source/" + fileName, JsonConvert.SerializeObject(tobaccoList));
        }
        
    }
}