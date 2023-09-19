using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Lobby.Shop
{
    using Visual;
    using Container;
    using Inventory;
    using Game.Inventory;
    using Save;
    using UI;

    public class ShopManager : MonoBehaviour, IService, IEntryComponent<LobbyInventory, UIWindowService>
    {
        public const WindowID SHOP_WINDOW_ID = WindowID.Shop;

        [Header("Buy")]
        [SerializeField, Expandable] private List<ShopProductListContainer> BuyProductListContainers;

        public ShopManagerVisual Visual { get; private set; }

        private LobbyInventory _inventory;

        public void Initialize(LobbyInventory inventory, UIWindowService windowService)
        {
            _inventory = inventory;

            List<ItemData> items = SaveManager.SessionSaveData.LobbyInventory.GetItemList();

            Visual = windowService.RegisterWindow<ShopManagerVisual>(SHOP_WINDOW_ID);
            Visual.Initialize(items, this);

            foreach (var container in BuyProductListContainers)
            {
                container.Initialize(this, Visual.AddProductContainerVisual(container));
            }

            _inventory.OnItemAdded += (data) =>
            {
                Visual.AddSellProductVisual(data);
            };
        }

        public void SellItem(InventoryItem item, int amount)
        {
            if(_inventory.TakeItem(new ItemData(item, amount)))
            {
                _inventory.AddMoney(item.Cost * amount);
            }
        }
    }
}
