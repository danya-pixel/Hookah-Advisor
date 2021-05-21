using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Hookah_Advisor.Parsers
{
    public class UserParser:IParser<User>
    {
        public void Write(Dictionary<int,User> database,string fileName)
        {
            var userList = database.Select(pair => pair.Value);
            File.WriteAllText("../../../" + fileName, JsonConvert.SerializeObject(userList));
        }
        
        public Dictionary<int, User> Load(string fileName)
        {
            if (!File.Exists("../../../" + fileName))
                return new Dictionary<int, User>();
            var str = File.ReadAllText("../../../" + fileName);
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