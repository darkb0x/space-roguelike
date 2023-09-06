using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Lobby.Shop.Visual
{
    using Lobby.Inventory;
    using Game.Inventory;

    public class ShopSellProductVisual : MonoBehaviour
    {
        [System.Serializable]
        public struct ButtonData
        {
            public Button Button;
            public TextMeshProUGUI ButtonText;
            [Space]
            public int SellAmount;
        }

        [SerializeField] private Image ItemIconImage;
        [SerializeField] private TextMeshProUGUI ItemCostText;
        [SerializeField] private TextMeshProUGUI ItemAmountText;
        [Space]
        [SerializeField] private ButtonData[] Buttons = new ButtonData[2];

        public InventoryItem Item { get; private set; }
        private ShopManager shopManager;
        private ShopManagerVisual managerVisual;
        private LobbyInventory LobbyInventory;

        public void Initialize(ItemData itemData, ShopManager manager)
        {
            LobbyInventory = ServiceLocator.GetService<LobbyInventory>();

            Item = itemData.Item;
            shopManager = manager;
            managerVisual = manager.Visual;

            ItemIconImage.sprite = Item.Icon;
            ItemCostText.text = Item.Cost+"$";
            ItemAmountText.text = itemData.Amount.ToString();

            foreach (var button in Buttons)
            {
                button.ButtonText.text = $"Sell ({button.SellAmount})";

                button.Button.onClick.AddListener(() => 
                    Sell(button.SellAmount)
                );
            }
        }

        public void UpdateVisual()
        {
            ItemData data = LobbyInventory.GetItem(Item);

            ItemAmountText.text = data.Amount.ToString();

            foreach (var button in Buttons)
            {
                if(data.Amount < button.SellAmount)
                {
                    button.Button.interactable = false;
                    continue;
                }
                button.Button.interactable = true;
            }
        }

        public void Sell(int amount)
        {
            shopManager.SellItem(Item, amount);

            managerVisual.UpdateProductsVisual();
        }
    }
}
