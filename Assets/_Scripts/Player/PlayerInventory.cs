using AYellowpaper.SerializedCollections;

namespace Game.Player
{
    using Inventory;

    public class PlayerInventory : Inventory, IService, IEntryComponent
    {
        public virtual void Initialize()
        {
            Load();
        }
        protected override void Load()
        {
            Money = _currentSessionData.Money;

            Items = new SerializedDictionary<InventoryItem, int>();
            foreach (var item in _currentSessionData.MainInventory.GetItemList())
            {
                AddItem(item, false);
            }
        }

        public override void AddItem(ItemData item, bool showNotify = true)
        {
            base.AddItem(item, showNotify);
            _currentSessionData.MainInventory.UpdateItemData(GetItem(item.Item));
        }
        public override bool TakeItem(ItemData item, bool showNotify = true)
        {
            if(base.TakeItem(item, showNotify))
            {
                _currentSessionData.MainInventory.UpdateItemData(GetItem(item.Item));
                return true;
            }
            return false;
        }
    }
}
