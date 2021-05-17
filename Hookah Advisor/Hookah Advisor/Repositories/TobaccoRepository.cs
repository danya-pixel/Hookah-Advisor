using System;
using System.Collections.Generic;
using System.Linq;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor.Repositories
{
    public class TobaccoRepository : IItemRepository<Tobacco>
    {
        private readonly Dictionary<int, Tobacco> _tobaccoDatabase = new()
        {
            {
                1,
                new Tobacco(1, "Банановое говно", "banana mama with shit's smell", new List<string> {"Фрукты", "говно"},
                    new List<string> {"Банан", "говно"})
            }
        };


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

        public List<Tobacco> SearchTobaccoInDict(string userRequest)
        {
            var tobaccoFromRequest = new List<Tobacco>();
            Console.WriteLine("начинаю поиск");
            foreach (var (id, tobacco) in _tobaccoDatabase)
            {
                if (tobacco.tastes.Contains(userRequest))
                {
                    tobaccoFromRequest.Append(tobacco);
                    Console.WriteLine(tobacco.name);
                }
            }

            Console.WriteLine("сейчас кину список че нашёл");
            return tobaccoFromRequest;
        }

        public List<Tobacco> RecommendTobacco()
        {
            //холодок


            //сладкий
            return null;
        }
    }
}