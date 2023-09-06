using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Lobby.Shop.Container
{
    using Visual;
    using Game.Inventory;

    public abstract class Product
    {
        public string Name;
        [ResizableTextArea] public string Description;
        [Space]
        public Sprite Icon;
        public int Cost;
    }

    public abstract class ShopProductListContainer : ScriptableObject
    {
        [Header("Category")]
        public string CategoryName = "Product Category";
        [ShowAssetPreview] public Sprite CategoryIcon;

        protected ShopManager shopManager;
        protected ShopProductListContainerVisual visual;
        [HideInInspector] public List<Product> products = new List<Product>();

        public virtual void Initialize(ShopManager manager, ShopProductListContainerVisual containerVisual)
        {
            shopManager = manager;
            visual = containerVisual;

            containerVisual.Initialize(this, manager.Visual);
        }

        public virtual void Buy(Product product, ShopBuyProductVisual productVisual)
        {
            ServiceLocator.GetService<PlayerInventory>().TakeMoney(product.Cost);
        }
    }
}
