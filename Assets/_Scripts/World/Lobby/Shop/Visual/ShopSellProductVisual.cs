using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Lobby.Shop.Visual
{
    using Player.Inventory;
    using Lobby.Inventory;

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

        public void Initialize(InventoryItem item, int amount, ShopManager manager)
        {
            LobbyInventory = Singleton.Get<LobbyInventory>();

            Item = item;
            shopManager = manager;
            managerVisual = manager.Visual;

            ItemIconImage.sprite = item.Icon;
            ItemCostText.text = item.Cost+"$";
            ItemAmountText.text = amount.ToString();

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
