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

    public class ShopManager : MonoBehaviour
    {
        [Header("Buy")]
        [SerializeField, Expandable] private List<ShopProductListContainer> BuyProductListContainers;

        [Header("Visual")]
        public ShopManagerVisual Visual;

        private PlayerInventory mainInventory;
        private LobbyInventory lobbyInventory;

        private void Start()
        {
            mainInventory = Singleton.Get<PlayerInventory>();
            lobbyInventory = Singleton.Get<LobbyInventory>();

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
            if(lobbyInventory.TakeItem(item, amount))
            {
                mainInventory.money += item.Cost * amount;
            }
        }
    }
}
