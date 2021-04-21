using System.Collections.Generic;

namespace Hookah_Advisor.Repository_Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User GetUserById(int userId);
        void AddUserById(int userId, string userName);
        void DeleteUserById(int userId); // пока не знаю насколько нужно
        void UpdateUsername(int userId, string newUserName);
        void SaveToJson();
        void LoadFromJson();
    }
}