using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Lobby.Shop.Visual
{
    using Game.Inventory;
    using Container;
    using Container.Visual;
    using Input;
    using System.Linq;
    using UI;

    public class ShopManagerVisual : Window
    {
        [Header("Sell")]
        [SerializeField] private ShopSellProductVisual SellProductVisual;
        [SerializeField] private Transform SellProductParent;

        [Header("Buy")]
        [SerializeField] private ShopProductListContainerVisual ProductListContainerVisual;
        [SerializeField] private Transform ProductListContainerVisualParent;

        [Header("Category")]
        [SerializeField] private GameObject CategoryButtonPrefab;
        [SerializeField] private Transform CategoryButtons;

        public override WindowID ID => ShopManager.SHOP_WINDOW_ID; 

        public bool isOpened { get; private set; }
        private ShopManager manager;

        private List<ShopSellProductVisual> sellProductVisuals = new List<ShopSellProductVisual>();
        private List<ShopProductListContainerVisual> productContainerVisuals = new List<ShopProductListContainerVisual>();
        private List<Image> categoryButtonVisuals = new List<Image>();

        private UIInputHandler _input => InputManager.UIInputHandler;

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            _input.CloseEvent += _closeAction;
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            _input.CloseEvent -= _closeAction; 
        }

        public void Initialize(List<ItemData> items, ShopManager manager)
        {
            this.manager = manager;

            foreach (var itemData in items)
            {
                AddSellProductVisual(itemData);
            }
        }

        public void AddSellProductVisual(ItemData itemData)
        {
            var productVisual = sellProductVisuals.FirstOrDefault(x => x.Item == itemData.Item);
            if (productVisual != null)
            {
                productVisual.UpdateVisual();
                return;
            }

            ShopSellProductVisual visual = Instantiate(SellProductVisual.gameObject, SellProductParent).GetComponent<ShopSellProductVisual>();
            visual.Initialize(itemData, manager);
            visual.OnChanged += UpdateProductsVisual;

            sellProductVisuals.Add(visual);
        }

        public ShopProductListContainerVisual AddProductContainerVisual(ShopProductListContainer container)
        {
            ShopProductListContainerVisual visual = Instantiate(ProductListContainerVisual.gameObject, ProductListContainerVisualParent).GetComponent<ShopProductListContainerVisual>();

            GameObject categoryButtonObj = Instantiate(CategoryButtonPrefab, CategoryButtons);
            Image categoryButtonVisual = categoryButtonObj.GetComponent<Image>();
            Image categotyButtonIcon = categoryButtonObj.transform.GetChild(0).GetComponent<Image>();
            Button categoryButton = categoryButtonObj.GetComponent<Button>();

            categotyButtonIcon.sprite = container.CategoryIcon;
            categoryButtonObj.GetComponent<Button>().onClick.AddListener(() => {
                OpenProductContainer(visual, categoryButtonVisual);
            });

            categoryButtonVisuals.Add(categoryButtonVisual);
            productContainerVisuals.Add(visual);

            return visual;
        }
        public void OpenProductContainer(ShopProductListContainerVisual containerVisual, Image senderVisual)
        {
            foreach (var visual in productContainerVisuals)
            {
                visual.gameObject.SetActive(false);
            }
            foreach (var visual in categoryButtonVisuals)
            {
                visual.color = new Color(0, 0, 0, 0); // transparent black
            }
            senderVisual.color = new Color(0, 0, 0, 0.3921569f);
            containerVisual.gameObject.SetActive(true);
        }

        public void UpdateProductsVisual()
        {
            foreach (var visual in sellProductVisuals)
            {
                visual.UpdateVisual();
            }
            foreach (var containerVisual in productContainerVisuals)
            {
                containerVisual.UpdateVisual();
            }
        }

        public override void Open(bool notify = true)
        {
            UpdateProductsVisual();

            OpenProductContainer(productContainerVisuals[0], categoryButtonVisuals[0]);
            base.Open(notify);
        }
    }
}
