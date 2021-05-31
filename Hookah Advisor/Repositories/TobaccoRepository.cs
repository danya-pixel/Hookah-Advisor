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
            _tobaccoDatabase = tobaccoParser.Load("tobaccoDatabase.json");
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
            _tobaccoParser.Write(_tobaccoDatabase, "tobaccoDatabase_test.json");
        }

        public List<Tobacco> SearchItemInDict(string userRequest)
        {
            return _tobaccoDatabase.Values.Where(
                tobacco => tobacco.Tastes.Any(tobaccoTaste
                               => userRequest.Split(' ').Any(s => s.Length > 2 && tobaccoTaste.StartsWith(s))) ||
                           tobacco.Brand.ToLower().Contains(userRequest)
            ).ToList();
        }

        public List<Tobacco> RecommendTobacco()
        {
            return null;
        }

        public int GetRepositorySize()
        {
            return _tobaccoDatabase.Count;
        }
    }
}