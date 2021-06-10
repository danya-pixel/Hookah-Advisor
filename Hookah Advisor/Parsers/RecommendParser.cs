using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Hookah_Advisor.Parsers
{
    public class RecommendParser : IParser<Option>
    {
        public Dictionary<int, Option> Load(string fileName)
        {
            var str = File.ReadAllText("Source/" + fileName);
            var optionList = JsonConvert.DeserializeObject<List<Option>>(str);

            var optionDict = new Dictionary<int, Option>();
            if (optionList == null) return optionDict;

            foreach (var option in optionList)
            {
                optionDict[option.QuestionNumber] = option;
            }

            return optionDict;
        }

        public void Write(Dictionary<int, Option> database, string fileName)
        {
            List<Option> options = new List<Option>();
            foreach (var quest in database)
            {
                options.Add(quest.Value);
            }

            File.WriteAllText("Source/" + fileName, JsonConvert.SerializeObject(options));
        }
    }
}