using System.Collections.Generic;

namespace Hookah_Advisor.Repository_Interfaces
{
    public interface IItemRepository<T>
    {
        T GetItemById(int itemId);
        void Save();
        List<T> SearchItemInDict(string toLower);
        public int GetRepositorySize();
    }
}