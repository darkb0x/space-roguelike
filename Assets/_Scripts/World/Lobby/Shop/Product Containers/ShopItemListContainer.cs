using Game.Lobby.Shop.Container.Visual;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Lobby.Shop.Container
{
    using Player.Inventory;
    using Lobby.Inventory;

    [System.Serializable]
    public class ItemProduct : Product
    {
        [Space]
        public InventoryItem Item;
    }

    [CreateAssetMenu(fileName = "Shop Item List Container", menuName = "Game/Shop/new Item List Container")]
    public class ShopItemListContainer : ShopProductListContainer
    {
        [Header("Data")]
        [SerializeField] private List<ItemProduct> Items = new List<ItemProduct>();

        public override void Initialize(ShopManager manager, ShopProductListContainerVisual containerVisual)
        {
            foreach (var item in Items)
            {
                if (products.Contains(item))
                    continue;

                products.Add(item);
            }

            base.Initialize(manager, containerVisual);
        }

        public override void Buy(Product product, ShopBuyProductVisual productVisual)
        {
            base.Buy(product, productVisual);

            ItemProduct itemProduct = product as ItemProduct;

            LobbyInventory.Instance.AddItem(new ItemData(itemProduct.Item, 1));
        }
    }
}
