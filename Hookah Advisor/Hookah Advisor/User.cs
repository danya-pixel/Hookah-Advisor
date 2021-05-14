using System;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor
{
    public class User
    {
        private int id;
        private string userName { get; set; }

        private Condition condition;

        public User(int id, string userName)
        {
            this.id = id;
            this.userName = userName;
            condition = new Condition(userCondition.none, 0);
        }

        public void SetUserName(string newUserName)
        {
            userName = newUserName;
        }

        public void SetUserCondition(userCondition userCondition)
        {
            condition.SetCondition(userCondition);
        }

        public void SetUserQuestionNumber(int questionNumber)
        {
            condition.SetQuestionNumber(questionNumber);
        }

        public string GetUserName()
        {
            return userName;
        }
    }
}