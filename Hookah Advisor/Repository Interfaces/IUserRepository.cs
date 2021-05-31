using System.Collections.Generic;

namespace Hookah_Advisor.Repository_Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User GetUserById(int userId);
        void AddUserById(int userId, string userName);
        public void Save();
        Condition GetUserCondition(int userId);
        void UpdateUserQuestionNumber(int userId, int p1);
        void UpdateUserCondition(int userId, UserCondition search);
        bool IsUserRegistered(int userId);
    }
}