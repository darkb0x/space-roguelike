using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Game.Player
{
    using Inventory;
    using Input;
    using Game.UI;
    using UnityEngine.UI;

    public class PlayerInventory : Inventory, IService, IEntryComponent<UIWindowService>
    {
        private PlayerInputHandler _input => InputManager.PlayerInputHandler;

        private InventoryWindow _window;
        private bool _initialized;

        public virtual void Initialize(UIWindowService windowService)
        {
            _window = windowService.RegisterWindow<InventoryWindow>(WindowID.Inventory);

            Load();

            _input.InventoryEvent += OpenClose;

            _initialized = true;
        }
        private void OnDestroy()
        {
            if(_initialized)
                _input.InventoryEvent -= OpenClose;
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

        public void OpenClose()
        {
            _window.OpenClose();
        }
    }
}
