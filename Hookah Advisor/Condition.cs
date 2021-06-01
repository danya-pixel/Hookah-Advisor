namespace Hookah_Advisor
{
    public enum UserCondition
    {
        None,
        Search,
        Recommendation,
    }

    public class Condition
    {
        public UserCondition CurrentCondition;
        public int QuestionNumber;

        public Condition(UserCondition currentCondition, int questionNumber)
        {
            CurrentCondition = currentCondition;
            QuestionNumber = questionNumber;
        }

        public void SetCondition(UserCondition condition)
        {
            CurrentCondition = condition;
        }
        
        public void SetQuestionNumber(int questionNumber)
        {
            QuestionNumber = questionNumber;
        }
        
        public UserCondition GetCondition()
        {
            return CurrentCondition;
        }
        
        public int GetQuestionNumber()
        {
            return QuestionNumber;
        }
    }
}