using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hookah_Advisor.Repository_Interfaces;
using Newtonsoft.Json;

namespace Hookah_Advisor.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static readonly Dictionary<int, User> UsersByIdDict = new();

        public void AddUserById(int userId, string userName)
        {
            var newUser = new User(userId, userName);
            if (!IsUserRegistered(userId))
                UsersByIdDict[userId] = newUser;
            Console.WriteLine($"User {userName} has been added");
        }

        public void DeleteUserById(int userId)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();
            UsersByIdDict.Remove(userId);
        }

        public void UpdateUsername(int userId, string newUserName)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();

            GetUserById(userId).SetUserName(newUserName);
        }

        public void UpdateUserCondition(int userId, userCondition condition)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();
            
            GetUserById(userId).SetUserCondition(condition);
        }
        
        public void UpdateUserQuestionNumber(int userId, int questionNumber)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();
            
            GetUserById(userId).SetUserQuestionNumber(questionNumber);
        }
        
        public bool IsUserRegistered(int userId)
        {
            return UsersByIdDict.ContainsKey(userId);
        }

        public IEnumerable<User> GetUsers()
        {
            return UsersByIdDict.Cast<User>();
        }

        public User GetUserById(int userId)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();
            return UsersByIdDict[userId];
        }

        public Condition GetUserCondition(int userId)
        {
            return GetUserById(userId).GetUserCondition();
        }
        
        /// <summary>
        /// Метод, обрабатывающий событие, когда репозиторий обращается по userId
        /// к пользователю, которого не существует
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private static void InvalidUserHandler()
        {
            throw new ArgumentException("User does not exist");
        }

        public void SaveToJson(string fileName)
        {
            var convertedDictionary = UsersByIdDict.ToDictionary(item => item.Key, item => item.Value.GetUserName());
            using var file = File.CreateText("../../../" + fileName);
            var serializer = new JsonSerializer();
            serializer.Serialize(file, convertedDictionary);
        }

        public void LoadFromJson(string fileName)
        {
            var str = File.ReadAllText("../../../" + fileName);
            var usersDict = JsonConvert.DeserializeObject<Dictionary<int,string>>(str);
            if (usersDict == null) return;
            
            foreach (var (id, name) in usersDict)
            {
                AddUserById(id, name);
            }
        }
    }
}