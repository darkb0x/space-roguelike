using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Game.Inventory
{
    using Input;

    public class PlayerInventory : Inventory, IService, IEntryComponent
    {
        [SerializeField] private InventoryWindow Window;

        private PlayerInputHandler _input => InputManager.PlayerInputHandler;

        public virtual void Initialize()
        {
            Load();
            _input.InventoryEvent += Window.Open;
        }
        private void OnDisable()
        {
            _input.InventoryEvent -= Window.Open;
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
