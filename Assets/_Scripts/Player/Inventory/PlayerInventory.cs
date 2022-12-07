using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Player.Inventory
{
    using CraftSystem;

    public class PlayerInventory : MonoBehaviour
    {
        // singletone
        public static PlayerInventory playerInventory;
        private void Awake() => playerInventory = this;

        [System.Serializable]
        public class Item
        {
            [NaughtyAttributes.Expandable] public InventoryItem item;
            public int amount;

            [HideInInspector] public Image UI_icon;
            [HideInInspector] public TextMeshProUGUI UI_amount;

            public void Add(int value)
            {
                this.amount += value;
            }
            public void Take(int value)
            {
                this.amount -= value;
            }
            public void SetUI(Image img, TextMeshProUGUI text)
            {
                UI_icon = img;
                UI_amount = text;
            }
        }

        [Header("Inventory")]
        public List<Item> items = new List<Item>();
        public int money
        {
            get
            {
                return m_money;
            }
            set
            {
                m_money = value;
            }
        }
        [SerializeField] private int m_money;

        [Header("UI")]
        [SerializeField] private Transform uiElements_tranform;
        [SerializeField] private GameObject itemInUI_prefab;

        private void Start()
        {
            for (int i = 0; i < items.Count; i++)
            {
                Transform obj = Instantiate(itemInUI_prefab, uiElements_tranform).transform;
                items[i].SetUI(obj.GetChild(0).GetComponent<Image>(), obj.GetChild(1).GetComponent<TextMeshProUGUI>());
            }
            UpdateUI();
        }

        public Item GetItem(InventoryItem item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == item)
                {
                    return items[i];
                }
            }
            return null;
        }

        public void AddItem(InventoryItem item, int amount)
        {
            GetItem(item).Add(amount);

            UpdateUI();
        }

        public bool TakeItem(InventoryItem item, int amount)
        {
            Item it = GetItem(item);
            if (it.amount >= amount)
            {
                it.Take(amount);
                UpdateUI();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanTakeItems(List<ItemCraft> items)
        {
            foreach (ItemCraft item in items)
            {
                if (GetItem(item.item).amount < item.amount)
                    return false;
            }
            return true;
        }

        private void UpdateUI()
        {
            foreach (var item in items)
            {
                item.UI_icon.sprite = item.item._icon;
                item.UI_amount.text = item.amount.ToString();
            }
        }
    }
}
