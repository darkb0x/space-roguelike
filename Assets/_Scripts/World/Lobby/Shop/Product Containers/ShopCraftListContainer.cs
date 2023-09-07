using Game.Lobby.Shop.Container.Visual;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Lobby.Shop.Container
{
    using Game.CraftSystem.Research;
    using global::CraftSystem.ScriptableObjects;
    using SaveData;

    
    [System.Serializable]
    public class CraftProduct : Product
    {
        [Space]
        [NaughtyAttributes.Expandable] public CraftSO Craft;
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

                if(!SaveDataManager.Instance.CurrentSessionData.ContainsCraft(craft.Craft)) 
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

            // TO DO
            ServiceLocator.GetService<ResearchManager>().Research(craftProduct.Craft);

            productVisual.Interactable = false;
        }
    }
}
