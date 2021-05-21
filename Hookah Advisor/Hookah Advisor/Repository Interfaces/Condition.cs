namespace Hookah_Advisor.Repository_Interfaces
{
    public enum userCondition
    {
        none,
        search,
        recommendation,
    }

    public class Condition
    {
        public userCondition condition = userCondition.none;
        private int questionNumber = 0;

        public Condition(userCondition condition, int questionNumber)
        {
            this.condition = condition;
            this.questionNumber = questionNumber;
        }

        public void SetCondition(userCondition condition)
        {
            this.condition = condition;
        }
        
        public void SetQuestionNumber(int questionNumber)
        {
            this.questionNumber = questionNumber;
        }
        
        public userCondition GetCondition()
        {
            return condition;
        }
        
        public int GetQuestionNumber()
        {
            return questionNumber;
        }
    }
}