using System.Collections.Generic;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor.Repositories
{
    public class TobaccoRepository : ITobaccoRepository
    {
        private readonly Dictionary<int, Tobacco> _tobaccoDatabase;

        public TobaccoRepository()
        {
            // parse Json
        }
        
        public Tobacco GetTobaccoById(int tobaccoId)
        {
            throw new System.NotImplementedException();
        }

        public void AddTobacco(Tobacco tobacco)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteTobacco(Tobacco tobacco)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateTobacco(Tobacco tobacco)
        {
            throw new System.NotImplementedException();
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }
    }
}