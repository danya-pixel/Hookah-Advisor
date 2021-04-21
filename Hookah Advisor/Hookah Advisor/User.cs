namespace Hookah_Advisor
{
    public class User
    {
        private int id;
        private string userName { get; set; }

        public User(int id, string userName)
        {
            this.id = id;
            this.userName = userName;
        }

        public void SetUserName(string newUserName)
        {
            userName = newUserName;
        }

        public string GetUserName()
        {
            return userName;
        }
    }
}