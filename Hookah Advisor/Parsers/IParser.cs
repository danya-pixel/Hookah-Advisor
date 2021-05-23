using System.Collections.Generic;

namespace Hookah_Advisor.Parsers
{
    public interface IParser<T>
    {
        public void Write(Dictionary<int, T> database,string fileName);
        public Dictionary<int, T> Load(string fileName);
    }
}