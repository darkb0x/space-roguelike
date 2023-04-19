using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Lobby.Shop.Visual
{
    using Player.Inventory;
    using Container;
    using Container.Visual;

    public class ShopManagerVisual : MonoBehaviour, IUIPanelManagerObserver
    {
        [SerializeField] private GameObject MainPanel;

        [Header("Sell")]
        [SerializeField] private ShopSellProductVisual SellProductVisual;
        [SerializeField] private Transform SellProductParent;

        [Header("Buy")]
        [SerializeField] private ShopProductListContainerVisual ProductListContainerVisual;
        [SerializeField] private Transform ProductListContainerVisualParent;

        [Header("Category")]
        [SerializeField] private GameObject CategoryButtonPrefab;
        [SerializeField] private Transform CategoryButtons;

        public bool isOpened { get; private set; }
        private ShopManager manager;

        private List<ShopSellProductVisual> sellProductVisuals = new List<ShopSellProductVisual>();
        private List<ShopProductListContainerVisual> productContainerVisuals = new List<ShopProductListContainerVisual>();
        private List<Image> categoryButtonVisuals = new List<Image>();

        private void Start()
        {
            manager = GetComponent<ShopManager>();

            GameInput.InputActions.UI.CloseWindow.performed += ClosePanel;
            UIPanelManager.Instance.Attach(this);
        }

        public void Initialize(List<ItemData> items)
        {
            foreach (var itemData in items)
            {
                AddSellProductVisual(itemData);
            }
        }

        public void AddSellProductVisual(ItemData itemData)
        {
            ShopSellProductVisual visual = Instantiate(SellProductVisual.gameObject, SellProductParent).GetComponent<ShopSellProductVisual>();
            visual.Initialize(itemData.Item, itemData.Amount, manager);

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

        public void OpenPanel()
        {
            UpdateProductsVisual();

            OpenProductContainer(productContainerVisuals[0], categoryButtonVisuals[0]);

            UIPanelManager.Instance.OpenPanel(MainPanel);
            isOpened = true;
        }

        public void ClosePanel()
        {
            if (!isOpened)
                return;

            UIPanelManager.Instance.ClosePanel(MainPanel);
            isOpened = false;
        }
        public void ClosePanel(InputAction.CallbackContext callback)
        {
            ClosePanel();
        }

        private void OnDisable()
        {
            GameInput.InputActions.UI.CloseWindow.performed -= ClosePanel;
        }

        public void PanelStateIsChanged(GameObject panel)
        {
            if(panel != MainPanel)
            {
                if(isOpened)
                {
                    isOpened = false;
                }
            }
        }
    }
}
