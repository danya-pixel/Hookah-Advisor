using System;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        private Condition condition;

        public User(int id, string userName)
        {
            Id = id;
            UserName = userName;
            condition = new Condition(userCondition.none, 0);
        }

        public void SetUserName(string newUserName)
        {
            UserName = newUserName;
        }

        public void SetUserCondition(userCondition userCondition)
        {
            condition.SetCondition(userCondition);
        }

        public void SetUserQuestionNumber(int questionNumber)
        {
            condition.SetQuestionNumber(questionNumber);
        }

        public Condition GetUserCondition()
        {
            return condition;
        }

        public int GetUserQuestionNumber()
        {
            return condition.GetQuestionNumber();
        }
    }
}