using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;


namespace Hookah_Advisor.Parsers
{
    public class UserParser : IParser<User>
    {
        public void Write(Dictionary<int, User> database, string fileName)
        {
            var userList = database.Select(pair => pair.Value);
            var path = "../../../Source/" + fileName;
            File.WriteAllText(path, JsonConvert.SerializeObject(userList));
        }

        public Dictionary<int, User> Load(string fileName)
        {
            var path = "../../../Source/" + fileName;
            if (!File.Exists(path))
                return new Dictionary<int, User>();
            var str = File.ReadAllText(path);
            var userList = JsonConvert.DeserializeObject<List<User>>(str);

            var userDict = new Dictionary<int, User>();
            if (userList == null) return userDict;

            foreach (var user in userList)
            {
                userDict[user.Id] = user;
            }

            return userDict;
        }
    }
}