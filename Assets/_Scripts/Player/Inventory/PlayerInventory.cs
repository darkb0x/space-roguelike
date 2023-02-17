using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Player.Inventory
{
    using CraftSystem;

    public interface IInventoryObserver
    {
        public void UpdateData(PlayerInventory inventory);
    }

    public class PlayerInventory : MonoBehaviour
    {
        // singletone
        public static PlayerInventory instance;
        private void Awake() => instance = this;

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
        public List<IInventoryObserver> observers = new List<IInventoryObserver>();

        public int money
        {
            get
            {
                return m_money;
            }
            set
            {
                m_money = value;
                foreach (var text in money_texts)
                {
                    text.text = m_money + "$";
                }
            }
        }
        [SerializeField] private int m_money;
        [SerializeField] private TextMeshProUGUI[] money_texts;

        [Header("UI")]
        [SerializeField] private Transform uiElements_tranform;
        [SerializeField] private GameObject itemInUI_prefab;
        [Space]
        [SerializeField] private InventoryScreen[] screens;

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

        /// <summary>
        /// Give an item to inventory
        /// </summary>
        /// <param name="item">item to give</param>
        /// <param name="amount">amount</param>
        public void GiveItem(InventoryItem item, int amount)
        {
            GetItem(item).Add(amount);

            UpdateUI();
        }

        /// <summary>
        /// Take item from inventory
        /// </summary>
        /// <param name="item">item to take</param>
        /// <param name="amount">amount</param>
        /// <returns>return false if you cannot take item, true if you can and take it</returns>
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
            // main
            foreach (var item in items)
            {
                item.UI_icon.sprite = item.item.Icon;
                item.UI_amount.text = item.amount.ToString();
            }

            // screens data
            foreach (var screen in screens)
            {
                screen.UpdateData(items);
            }

            // observers data
            foreach (var observer in observers)
            {
                observer.UpdateData(this);
            }
        }
    }
}
