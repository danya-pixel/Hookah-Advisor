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
        public UserCondition _condition;
        public int _questionNumber;

        public Condition(UserCondition condition, int questionNumber)
        {
            _condition = condition;
            _questionNumber = questionNumber;
        }

        public void SetCondition(UserCondition condition)
        {
            _condition = condition;
        }
        
        public void SetQuestionNumber(int questionNumber)
        {
            _questionNumber = questionNumber;
        }
        
        public UserCondition GetCondition()
        {
            return _condition;
        }
        
        public int GetQuestionNumber()
        {
            return _questionNumber;
        }
    }
}