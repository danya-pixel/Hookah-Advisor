using System;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor
{
    public class User
    {
        public int Id { get; }
        public string UserName { get; set; } //should be public for Serializer

        private readonly Condition _condition;

        public User(int id, string userName)
        {
            Id = id;
            UserName = userName;
            _condition = new Condition(userCondition.none, 0);
        }

        public void SetUserName(string newUserName)
        {
            UserName = newUserName;
        }

        public void SetUserCondition(userCondition userCondition)
        {
            _condition.SetCondition(userCondition);
        }

        public void SetUserQuestionNumber(int questionNumber)
        {
            _condition.SetQuestionNumber(questionNumber);
        }

        public Condition GetUserCondition()
        {
            return _condition;
        }

        public int GetUserQuestionNumber()
        {
            return _condition.GetQuestionNumber();
        }
    }
}