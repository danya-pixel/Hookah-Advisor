namespace Hookah_Advisor.Repository_Interfaces
{
    public interface ITobaccoRepository
    {
        Tobacco GetTobaccoById(int tobaccoId);
        void AddTobacco(Tobacco tobacco);
        void DeleteTobacco(Tobacco tobacco); // пока не знаю насколько нужно
        void UpdateTobacco(Tobacco tobacco);
        void Save();
    }
}