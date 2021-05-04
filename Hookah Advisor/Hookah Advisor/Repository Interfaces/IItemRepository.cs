namespace Hookah_Advisor.Repository_Interfaces
{
    public interface IItemRepository<T>
    {
        T GetItemById(int itemId);
        void AddItem(T item);
        void DeleteItem(T item); 
        void UpdateItem(T item);
        void Save();
    }
}