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
        private static Dictionary<int, User> _usersByIdDict = new Dictionary<int, User>();

        public void AddUserById(int userId, string userName)
        {
            var newUser = new User(userId, userName);
            if (!IsUserRegistered(userId))
                _usersByIdDict[userId] = newUser;
            Console.WriteLine($"User {userName} has been added");
        }

        public void DeleteUserById(int userId)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();
            _usersByIdDict.Remove(userId);
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
            return _usersByIdDict.ContainsKey(userId);
        }

        public IEnumerable<User> GetUsers()
        {
            foreach (var (_, user) in _usersByIdDict)
            {
                yield return user;
            }
        }

        public User GetUserById(int userId)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();
            return _usersByIdDict[userId];
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

        public void SaveToJson()
        {
            var convertedDictionary = _usersByIdDict.ToDictionary(item => item.Key, item => item.Value.GetUserName());
            using var file = File.CreateText(@"\path"); //should be implemented
            var serializer = new JsonSerializer();
            serializer.Serialize(file, convertedDictionary);
        }

        public void LoadFromJson()
        {
            throw new NotImplementedException();
        }
    }
}