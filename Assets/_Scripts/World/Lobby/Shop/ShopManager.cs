using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Lobby.Shop
{
    using Visual;
    using Container;
    using Inventory;
    using Player.Inventory;
    using SaveData;

    public class ShopManager : MonoBehaviour, IService, IEntryComponent<PlayerInventory, LobbyInventory>
    {
        [Header("Buy")]
        [SerializeField, Expandable] private List<ShopProductListContainer> BuyProductListContainers;

        [Header("Visual")]
        public ShopManagerVisual Visual;

        private PlayerInventory _playerInventory;
        private LobbyInventory _lobbyInventory;

        public void Initialize(PlayerInventory playerInventory, LobbyInventory lobbyInventory)
        {
            _playerInventory = playerInventory;
            _lobbyInventory = lobbyInventory;

            List<ItemData> items = SaveDataManager.Instance.CurrentSessionData.LobbyInventory.GetItemList();
            Visual.Initialize(items);

            foreach (var container in BuyProductListContainers)
            {
                container.Initialize(this, Visual.AddProductContainerVisual(container));
            }

            lobbyInventory.OnNewItem += (data) =>
            {
                Visual.AddSellProductVisual(data);
            };
        }

        public void SellItem(InventoryItem item, int amount)
        {
            if(_lobbyInventory.TakeItem(item, amount))
            {
                _playerInventory.money += item.Cost * amount;
            }
        }
    }
}
