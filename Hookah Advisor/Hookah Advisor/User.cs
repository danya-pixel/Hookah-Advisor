using System;
using System.Collections.Generic;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor
{
    public class User
    {
        public int Id { get; }
        public string UserName { get; set; } //should be public for Serializer
        public HashSet<int> SmokingLater { get; set; } 

        public readonly Condition Condition;

        public User(int id, string userName)
        {
            Id = id;
            UserName = userName;
            Condition = new Condition(userCondition.none, 0);
            SmokingLater = new HashSet<int>();
        }

        public void SetUserName(string newUserName)
        {
            UserName = newUserName;
        }

        public void SetUserCondition(userCondition userCondition)
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