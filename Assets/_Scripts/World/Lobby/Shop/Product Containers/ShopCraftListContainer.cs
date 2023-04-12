using Game.Lobby.Shop.Container.Visual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Lobby.Shop.Container
{
    using CraftSystem.Editor.ScriptableObjects;
    using CraftSystem;
    using SaveData;

    [System.Serializable]
    public class CraftProduct : Product
    {
        [Space]
        [NaughtyAttributes.Expandable] public CSCraftSO Craft;
    }

    [CreateAssetMenu(fileName = "Shop Craft List Container", menuName = "Game/Shop/new Craft List Container")]
    public class ShopCraftListContainer : ShopProductListContainer
    {
        [Header("Data")]
        [SerializeField, NaughtyAttributes.ReorderableList] private List<CraftProduct> Crafts = new List<CraftProduct>();

        public override void Initialize(ShopManager manager, ShopProductListContainerVisual containerVisual)
        {
            foreach (var craft in Crafts)
            {
                if (products.Contains(craft))
                    continue;

                if(!GameData.Instance.CurrentSessionData.HaveCraft(craft.Craft)) 
                {
                    products.Add(craft);
                }
            }

            base.Initialize(manager, containerVisual);
        }
        public override void Buy(Product product, ShopBuyProductVisual productVisual)
        {
            base.Buy(product, productVisual);

            CraftProduct craftProduct = product as CraftProduct;

            LearnCSManager.Instance.LearnCraft(craftProduct.Craft);

            productVisual.Interactable = false;
        }
    }
}
