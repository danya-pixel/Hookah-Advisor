using System;
using System.Collections.Generic;
using System.Linq;
using Hookah_Advisor.Parsers;
using Hookah_Advisor.Repository_Interfaces;


namespace Hookah_Advisor.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Dictionary<int, User> _usersDatabase;
        private readonly IParser<User> _userParser;

        public UserRepository(IParser<User> userParser)
        {
            _userParser = userParser;
            _usersDatabase = userParser.Load("users.json");
        }

        public void AddUserById(int userId, string userName)
        {
            var newUser = new User(userId, userName);
            if (!IsUserRegistered(userId))
                _usersDatabase[userId] = newUser;
            Console.WriteLine($"User {userName} has been added");
        }

        public void DeleteUserById(int userId)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();
            _usersDatabase.Remove(userId);
        }

        public void UpdateUsername(int userId, string newUserName)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();

            GetUserById(userId).UserName = newUserName;
        }

        public void UpdateUserCondition(int userId, UserCondition condition)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();

            GetUserById(userId).Condition.UserConditionProp = condition;
        }

        public void UpdateUserQuestionNumber(int userId, int questionNumber)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();

            GetUserById(userId).Condition.QuestionNumber = questionNumber;
        }

        public bool IsUserRegistered(int userId)
        {
            return _usersDatabase.ContainsKey(userId);
        }

        public IEnumerable<User> GetUsers()
        {
            return _usersDatabase.Select(pair => pair.Value);
        }

        public User GetUserById(int userId)
        {
            if (!IsUserRegistered(userId))
                InvalidUserHandler();
            return _usersDatabase[userId];
        }

        public Condition GetUserCondition(int userId)
        {
            return GetUserById(userId).Condition;
        }

        public void Save()
        {
            _userParser.Write(_usersDatabase, "users.json");
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
    }
}