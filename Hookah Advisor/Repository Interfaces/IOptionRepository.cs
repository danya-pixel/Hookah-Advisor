namespace Hookah_Advisor.Repository_Interfaces
{
    public interface IOptionRepository<T>
    {
        public Option GetNextQuestion(int questNum, bool next);
    }
}