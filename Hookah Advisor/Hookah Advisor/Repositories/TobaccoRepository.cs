using System.Collections.Generic;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor.Repositories
{
    public class TobaccoRepository : IItemRepository<Tobacco>
    {
        private readonly Dictionary<int, Tobacco> _tobaccoDatabase;

        public TobaccoRepository()
        {
            // parse Json
        }
        
        public Tobacco GetItemById(int tobaccoId)
        {
            throw new System.NotImplementedException();
        }

        public void AddItem(Tobacco tobacco)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteItem(Tobacco tobacco)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateItem(Tobacco tobacco)
        {
            throw new System.NotImplementedException();
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }
    }
}