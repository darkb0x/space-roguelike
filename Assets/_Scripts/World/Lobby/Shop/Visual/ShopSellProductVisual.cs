using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Lobby.Shop.Visual
{
    using Lobby.Inventory;
    using Game.Inventory;
    using System;

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

        public Action OnChanged;
        public InventoryItem Item { get; private set; }

        private ShopManager _shopManager;
        private LobbyInventory _lobbyInventory;

        public void Initialize(ItemData itemData, ShopManager manager)
        {
            _lobbyInventory = ServiceLocator.GetService<LobbyInventory>();

            Item = itemData.Item;
            _shopManager = manager;

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
            ItemData data = _lobbyInventory.GetItem(Item);

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
            _shopManager.SellItem(Item, amount);

            OnChanged?.Invoke();
        }
    }
}
