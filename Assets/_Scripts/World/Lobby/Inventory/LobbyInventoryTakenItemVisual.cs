using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Lobby.Inventory.Visual
{
    using Game.Inventory;

    public class LobbyInventoryTakenItemVisual : MonoBehaviour
    {
        public Image ItemIconImage;
        public TextMeshProUGUI ItemAmountText;
        [Space]
        public Slider ItemSlider;
        public TextMeshProUGUI TakenItemAmountText;

        public ItemData ItemData { get; private set; }
        private LobbyInventory _inventory;

        private int _itemAmount => _inventory.GetItem(ItemData.Item).Amount;
        private int _maxTakenItemAmount { 
            get
            {
                if (_itemAmount < _inventory.MaxTakenItemsAmount)
                {
                    return _itemAmount;
                }
                else
                {
                    return _inventory.MaxTakenItemsAmount;
                }
            }
        }
        private int _freeSpace => _inventory.FreeItemsAmount;
        

        public void Initialize(InventoryItem item)
        {
            _inventory = ServiceLocator.GetService<LobbyInventory>();

            ItemData = new ItemData(item);

            ItemIconImage.sprite = item.Icon;
            ItemSlider.value = 0;
            ItemSlider.maxValue = _maxTakenItemAmount;

            UpdateData();
        }

        public void UpdateData()
        {
            if (_itemAmount == 0)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);

            if(ItemData.Amount <= 0)
            {
                ItemAmountText.text = _itemAmount.ToString();
            }
            else
            {
                ItemAmountText.text = $"{_itemAmount}<color=#{ColorUtility.ToHtmlStringRGBA(Color.red)}>-{ItemData.Amount}</color>";
            }

            TakenItemAmountText.text = $"({ItemData.Amount}/{Mathf.Clamp(_freeSpace, 0, _maxTakenItemAmount)})";
            ItemSlider.maxValue = _maxTakenItemAmount;
        }

        public void SetTakenItemsAmount(float amount)
        {
            AddTakenItemsAmount((int)amount - ItemData.Amount);
        }

        public void AddTakenItemsAmount(int value)
        {
            if(_inventory.TryPickItemIntoSession(ItemData.Item, value))
            {
                ItemData.Amount = Mathf.Clamp(ItemData.Amount + value, 0, _maxTakenItemAmount);

                UpdateData();
            }
            ItemSlider.SetValueWithoutNotify(ItemData.Amount);
        }
    }
}
