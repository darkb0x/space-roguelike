using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Lobby.Shop.Container.Visual
{
    using Game.Inventory;
    using Shop.Visual;
    using System;

    public class ShopBuyProductVisual : MonoBehaviour
    {
        [SerializeField] private Image ProductIconImage;
        [SerializeField] private TextMeshProUGUI ProductCostText;
        [SerializeField] private TextMeshProUGUI ProductDescriptionText;
        [Space]
        [SerializeField] private Button BuyButton;
        [Space]
        [SerializeField] private CanvasGroup CanvasGroup;
        [SerializeField] private GameObject PurchasedText;

        public Action OnChanged;

        private Inventory _inventory;
        private ShopProductListContainer _container;

        private bool m_interactable;
        public bool Interactable
        {
            get
            {
                return m_interactable;
            }
            set
            {
                m_interactable = value;

                CanvasGroup.alpha = value ? 1 : 0.5f;
                BuyButton.interactable = !value;
                PurchasedText.SetActive(!value);
            }
        }
        public Product Product { get; private set; }
        private Color defaulrCostTextColor;


        public void Initialize(Product product, ShopProductListContainer productListContainer)
        {
            _inventory = ServiceLocator.GetService<Inventory>();

            Product = product;
            _container = productListContainer;

            defaulrCostTextColor = ProductCostText.color;

            ProductIconImage.sprite = product.Icon;
            ProductCostText.text = product.Cost + "$";
            ProductDescriptionText.text = $"{product.Name}\n<size=15><color=#ABABAB>{product.Description}";

            BuyButton.onClick.AddListener(() =>
            {
                Buy();
            });

            UpdateVisual();
        }

        public void UpdateVisual()
        {
            if(_inventory.Money >= Product.Cost)
            {
                ProductCostText.color = defaulrCostTextColor;
                BuyButton.interactable = true;
            }
            else
            {
                ProductCostText.color = Color.red;
                BuyButton.interactable = false;
            }
        }

        public void Buy()
        {
            _container.Buy(Product, this);

            OnChanged?.Invoke();
        }
    }
}
