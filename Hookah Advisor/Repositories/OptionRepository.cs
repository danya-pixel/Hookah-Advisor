using System;
using System.Collections.Generic;
using Hookah_Advisor.Parsers;
using Hookah_Advisor.Repository_Interfaces;

namespace Hookah_Advisor.Repositories
{
    public class OptionRepository : IOptionRepository<Option>
    {
        private readonly Dictionary<int, Option> _optionData;

        public OptionRepository(IParser<Option> optionParser)
        {
            _optionData = optionParser.Load("optionList.json");
        }

        public Option GetItemById(int itemId)
        {
            try
            {
                return _optionData[itemId];
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
                return _optionData[questNum + 1];
            }

            return _optionData[questNum];
        }
    }
}