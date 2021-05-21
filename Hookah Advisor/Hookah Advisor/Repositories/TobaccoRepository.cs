using System;
using System.Collections.Generic;
using System.Linq;
using Hookah_Advisor.Parsers;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor.Repositories
{
    public class TobaccoRepository : IItemRepository<Tobacco>
    {
        private readonly Dictionary<int, Tobacco> _tobaccoDatabase;
        private readonly IParser<Tobacco> _tobaccoParser;

        public TobaccoRepository(IParser<Tobacco> tobaccoParser)
        {
            _tobaccoParser = tobaccoParser;
            _tobaccoDatabase = tobaccoParser.Load("table_v2.json");
        }


        public Tobacco GetItemById(int itemId)
        {
            try
            {
                return _tobaccoDatabase[itemId];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new KeyNotFoundException();
            }
        }

        public void Save()
        {
            _tobaccoParser.Write(_tobaccoDatabase, "table_v2_test.json");
        }

        public List<Tobacco> SearchTobaccoInDict(string userRequest)
        {
            var tobaccoFromRequest = new List<Tobacco>();
            foreach (var tobacco in _tobaccoDatabase.Values)
            {
                tobaccoFromRequest.AddRange(from t in tobacco.tastes where t.Contains(userRequest) select tobacco);
            }

            return tobaccoFromRequest;
        }

        public List<Tobacco> RecommendTobacco()
        {
            return null;
        }
    }
}