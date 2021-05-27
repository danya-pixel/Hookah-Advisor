using System.Collections.Generic;

namespace Hookah_Advisor.Repository_Interfaces
{
    public interface IItemRepository<out T>
    {
        T GetItemById(int itemId);
        void Save();
        List<Tobacco> SearchTobaccoInDict(string toLower);
    }
}