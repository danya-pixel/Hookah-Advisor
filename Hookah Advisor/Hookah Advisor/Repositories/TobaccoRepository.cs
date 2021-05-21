using System;
using System.Collections.Generic;
using System.Linq;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor.Repositories
{
    public class TobaccoRepository : IItemRepository<Tobacco>
    {
        private readonly Dictionary<int, Tobacco> _tobaccoDatabase;
        private readonly TableParser _tableParser;

        public TobaccoRepository()
        {
            _tableParser = new TableParser("table_v2.json");
            _tobaccoDatabase = _tableParser.GetTobaccosFromJson();
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
            _tableParser.WriteToJson(_tobaccoDatabase);
        }

        public List<Tobacco> SearchTobaccoInDict(string userRequest)
        {
            var tobaccoFromRequest = new List<Tobacco>();
            Console.WriteLine("начинаю поиск");
            foreach (var (id, tobacco) in _tobaccoDatabase)
            {
                if (tobacco.tastes.Contains(userRequest))
                {
                    tobaccoFromRequest.Add(tobacco);
                    Console.WriteLine(tobacco.name);
                }
            }

            Console.WriteLine("сейчас кину список че нашёл");
            return tobaccoFromRequest;
        }

        public List<Tobacco> RecommendTobacco()
        {
            return null;
        }
    }
}