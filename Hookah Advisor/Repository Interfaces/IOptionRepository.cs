namespace Hookah_Advisor.Repository_Interfaces
{
    public interface IOptionRepository<T>
    {
        public Option GetItemById(int itemId);
        public Option GetNextQuestion(int questNum, bool next);
    }
}