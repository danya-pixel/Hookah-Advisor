using System.Collections.Generic;

namespace Hookah_Advisor
{
    public class User
    {
        public int Id { get; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string UserName { get; set; } //should be public for Serializer
        public HashSet<int> SmokeLater { get; set; }
        public HashSet<int> SmokedHistory { get; set; }
        public readonly Condition Condition; //should be public for Serializer

        public User(int id, string userName)
        {
            Id = id;
            UserName = userName;
            Condition = new Condition(UserCondition.None, 0);
            SmokeLater = new HashSet<int>();
            SmokedHistory = new HashSet<int>();
        }

        public void SetUserName(string newUserName)
        {
            UserName = newUserName;
        }

        public void SetUserCondition(UserCondition userCondition)
        {
            Condition.SetCondition(userCondition);
        }

        public void SetUserQuestionNumber(int questionNumber)
        {
            Condition.SetQuestionNumber(questionNumber);
        }

        public Condition GetUserCondition()
        {
            return Condition;
        }

        public int GetUserQuestionNumber()
        {
            return Condition.GetQuestionNumber();
        }
    }
}