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
    }
}
