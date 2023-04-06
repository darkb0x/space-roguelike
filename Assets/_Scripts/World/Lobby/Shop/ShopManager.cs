using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Lobby.Shop
{
    using Visual;
    using Container;
    using Container.Visual;
    using Inventory;
    using Player.Inventory;
    using SaveData;
    using System;

    public class ShopManager : MonoBehaviour
    {
        [Header("Buy")]
        [SerializeField, Expandable] private List<ShopProductListContainer> BuyProductListContainers;

        [Header("Visual")]
        public ShopManagerVisual Visual;

        private PlayerInventory mainInventory => PlayerInventory.Instance;
        private LobbyInventory lobbyInventory => LobbyInventory.Instance;

        private void Start()
        {
            List<ItemData> items = GameData.Instance.CurrentSessionData.LobbyInventory.GetItemList();
            Visual.Initialize(items);

            foreach (var container in BuyProductListContainers)
            {
                container.Initialize(this, Visual.AddProductContainerVisual(container));
            }

            LobbyInventory.Instance.OnNewItem += (data) =>
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
