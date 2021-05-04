using System.Collections.Generic;
using System.Linq;
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


        public Tobacco GetItemById(int itemId)
        {
            throw new System.NotImplementedException();
        }

        public void AddItem(Tobacco item)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteItem(Tobacco item)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateItem(Tobacco item)
        {
            throw new System.NotImplementedException();
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }
        
        public List<Tobacco> SearchTobacco(string userRequest)
        {
            var tobaccoFromRequest = new List<Tobacco>();

            foreach (var (id, tobacco) in _tobaccoDatabase)
            {
                if (tobacco.tastes.Contains(userRequest))
                {
                    tobaccoFromRequest.Append(tobacco);
                }
            }

            return tobaccoFromRequest;
        }
    }
}