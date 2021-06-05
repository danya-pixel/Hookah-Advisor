namespace Hookah_Advisor.Repository_Interfaces
{
    public interface IRecommendation<T>
    {
        public Option GetItemById(int itemId);
        public Option GetNextQuestion(int questNum, bool next);
    }
}