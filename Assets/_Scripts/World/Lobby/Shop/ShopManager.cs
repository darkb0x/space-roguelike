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

    public class ShopManager : MonoBehaviour, IService, IEntryComponent<LobbyInventory>
    {
        [Header("Buy")]
        [SerializeField, Expandable] private List<ShopProductListContainer> BuyProductListContainers;

        [Header("Visual")]
        public ShopManagerVisual Visual;

        private LobbyInventory _inventory;

        public void Initialize(LobbyInventory inventory)
        {
            _inventory = inventory;

            List<ItemData> items = SaveManager.SessionSaveData.LobbyInventory.GetItemList();
            Visual.Initialize(items);

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
