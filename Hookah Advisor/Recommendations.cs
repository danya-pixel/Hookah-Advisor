using System;
using System.Collections.Generic;
using Hookah_Advisor.Parsers;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor
{
    public class Recommendations : IRecommendation<Option>
    {
        private readonly Dictionary<int, Option> optionData;
        private readonly IParser<Option> _optionParser;

        public Recommendations(IParser<Option> _optionParser)
        {
            this._optionParser = _optionParser;
            optionData = _optionParser.Load("option_list.json");
        }

        public Option GetItemById(int itemId)
        {
            try
            {
                return optionData[itemId];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new KeyNotFoundException();
            }
        }

        public Option GetNextQuestion(int questNum, bool next)
        {
            if (next)
            {
                return optionData[questNum + 1];
            }

            return optionData[questNum];
        }


        public void Save()
        {
            throw new NotImplementedException();
        }

        public List<Option> SearchTobaccoInDict(string toLower)
        {
            throw new NotImplementedException();
        }

        public int GetRepositorySize()
        {
            throw new NotImplementedException();
        }
    }
}