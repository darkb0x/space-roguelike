using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Lobby.Inventory.Visual
{
    using Player.Inventory;

    public class LobbyInventoryTakenItemVisual : MonoBehaviour
    {
        [SerializeField] private Image ItemIconImage;
        [SerializeField] private TextMeshProUGUI ItemAmountText;
        [Space]
        [SerializeField] private Slider ItemSlider;
        [SerializeField] private TextMeshProUGUI TakenItemAmountText;

        public ItemData ItemData { get; private set; }
        private LobbyInventory inventory;

        private int itemAmount => inventory.GetItem(ItemData.Item).Amount;
        private int maxTakenItemAmount { 
            get
            {
                if (itemAmount < inventory.MaxTakenItemsAmount)
                {
                    return itemAmount;
                }
                else
                {
                    return inventory.MaxTakenItemsAmount;
                }
            }
        }
        private int freeSpace => inventory.FreeItemsAmount;

        public void Initialize(ItemData itemdata, LobbyInventory lobbyInventory)
        {
            inventory = lobbyInventory;

            ItemData = new ItemData(itemdata.Item);

            ItemIconImage.sprite = ItemData.Item.Icon;
            ItemSlider.value = 0;
            ItemSlider.maxValue = maxTakenItemAmount;

            UpdateData();
        }

        public void UpdateData()
        {
            if(ItemData.Amount <= 0)
            {
                ItemAmountText.text = itemAmount.ToString();
            }
            else
            {
                ItemAmountText.text = $"{itemAmount}<color=#{ColorUtility.ToHtmlStringRGBA(Color.red)}>-{ItemData.Amount}</color>";
            }

            TakenItemAmountText.text = $"({ItemData.Amount}/{Mathf.Clamp(freeSpace, 0, maxTakenItemAmount)})";
            ItemSlider.maxValue = maxTakenItemAmount;
        }

        private void Update()
        {
            UpdateData();

            ItemSlider.value = Mathf.Clamp(ItemSlider.value, 0, maxTakenItemAmount);
            
            if (freeSpace < 0)
            {
                ItemData.Amount = Mathf.Clamp(ItemData.Amount - 1, 0, maxTakenItemAmount); ;
                ItemSlider.value = ItemData.Amount;
            }
        }

        public void SetTakenItemsAmount(float amount)
        {
            ItemData.Amount = Mathf.Clamp((int)amount, 0, maxTakenItemAmount);
        }

        public void AddTakenItemsAmount(int value)
        {
            ItemData.Amount = Mathf.Clamp(ItemData.Amount + value, 0, maxTakenItemAmount);
            ItemSlider.SetValueWithoutNotify(ItemData.Amount);
        }
    }
}
