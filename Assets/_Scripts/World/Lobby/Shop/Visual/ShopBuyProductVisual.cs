using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Lobby.Shop.Container.Visual
{
    using Player.Inventory;
    using Shop.Visual;

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

        private PlayerInventory playerInventory => PlayerInventory.Instance;
        private ShopProductListContainer container;
        private ShopManagerVisual managerVisual;

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


        public void Initialize(Product product, ShopProductListContainer productListContainer, ShopManagerVisual shopManagerVisual)
        {
            Product = product;
            container = productListContainer;
            managerVisual = shopManagerVisual;

            defaulrCostTextColor = ProductCostText.color;

            ProductIconImage.sprite = product.Icon;
            ProductCostText.text = product.Cost + "$";
            ProductDescriptionText.text = $"{product.Name}\n<size=17>{product.Description}</size>";

            BuyButton.onClick.AddListener(() =>
            {
                Buy();
            });

            UpdateVisual();
        }

        public void UpdateVisual()
        {
            if(playerInventory.money >= Product.Cost)
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
            container.Buy(Product, this);

            managerVisual.UpdateProductsVisual();
        }
    }
}
