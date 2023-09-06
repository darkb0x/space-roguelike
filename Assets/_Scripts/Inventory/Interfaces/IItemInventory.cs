using System.Collections.Generic;

namespace Game.Inventory
{
    public interface IItemInventory : IService
    {
        public ItemData GetItem(InventoryItem item);
        public List<ItemData> GetItems();
        public void AddItem(ItemData item, bool showNotify = true);
        public bool TakeItem(ItemData item, bool showNotify = true);
        public bool TakeItem(List<ItemData> items, bool showNotify = true);
    }
}
