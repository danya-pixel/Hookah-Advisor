namespace Hookah_Advisor.Repository_Interfaces
{
    public interface IOptionRepository<out T>
    { 
        public T GetNextQuestion(int questNum, bool next);
    }
}