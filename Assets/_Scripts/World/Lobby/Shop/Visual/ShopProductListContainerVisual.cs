using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Lobby.Shop.Container.Visual
{
    using Shop.Visual;

    public class ShopProductListContainerVisual : MonoBehaviour
    {
        [SerializeField] private ShopBuyProductVisual ProductVisual;
        [SerializeField] private Transform ProductVisualParent;

        private ShopProductListContainer productContainer;
        private List<ShopBuyProductVisual> productVisuals = new List<ShopBuyProductVisual>();

        public void Initialize(ShopProductListContainer container)
        {
            productContainer = container;
            foreach (var product in productContainer.products)
            {
                ShopBuyProductVisual visual = Instantiate(ProductVisual.gameObject, ProductVisualParent).GetComponent<ShopBuyProductVisual>();
                visual.Initialize(product, container);

                productVisuals.Add(visual);
            }
        }

        public void UpdateVisual()
        {
            foreach (var visual in productVisuals)
            {
                visual.UpdateVisual();
            }
        }
    }
}
