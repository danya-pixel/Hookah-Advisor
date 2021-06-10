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
        public UserCondition UserConditionProp { get; set; }
        public int QuestionNumber { get; set; }

        public Condition(UserCondition userConditionProp, int questionNumber)
        {
            UserConditionProp = userConditionProp;
            QuestionNumber = questionNumber;
        }
    }
}