using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

namespace Game.Player.Inventory
{
    using Visual;
    using SaveData;
    using Utilities.Notifications;
    using Input;

    public class PlayerInventory : MonoBehaviour, ISingleton
    {
        [Header("Inventory")]
        [ReadOnly] public List<ItemData> Items = new List<ItemData>();
        public int money
        {
            get
            {
                return m_money;
            }
            set
            {
                int currentMoney = m_money;
                m_money = value;

                int difference = Mathf.Abs(m_money - currentMoney);
                if (m_money < currentMoney)
                {
                    MoneyChangedText.text = "-" + difference;
                    MoneyChangedText.color = redColor;
                    MoneyChangedAnim.SetTrigger("moneyChanged");

                    LogUtility.WriteLog($"Player money = {currentMoney} - {difference}");
                }
                else if(m_money > currentMoney)
                {
                    MoneyChangedText.text = "+" + difference;
                    MoneyChangedText.color = greenColor;
                    MoneyChangedAnim.SetTrigger("moneyChanged");

                    LogUtility.WriteLog($"Player money = {currentMoney} + {difference}");
                }

                currentSessionData.Money = m_money;

                foreach (var text in money_texts)
                {
                    text.text = m_money + "$";
                }
            }
        }
        [SerializeField] private int m_money;

        [Header("Visual")]
        [SerializeField] private InventoryWindow InventoryWindow;
        [Space]
        [SerializeField] private TextMeshProUGUI[] money_texts;
        [SerializeField] private Animator MoneyChangedAnim;
        [SerializeField] private TextMeshProUGUI MoneyChangedText;

        public List<IInventoryObserver> observers = new List<IInventoryObserver>();
        private SessionData currentSessionData => SaveDataManager.Instance.CurrentSessionData;
        private PlayerInputHandler _input => InputManager.PlayerInputHandler;

        private Color greenColor = new Color(0.254902f, 0.8196079f, 0.5372549f, 1);
        private Color redColor = new Color(0.6901961f, 0.1098039f, 0.282353f, 1f);

        public bool IsActive { get; set; }

        private void Awake()
        {
            Singleton.Add(this);
            IsActive = true;
        }
        private void Start()
        {
            Load();

            UpdateVisual();
            
            _input.InventoryEvent += InventoryEnabled;
        }
        private void OnDisable()
        {
            _input.InventoryEvent -= InventoryEnabled;
        }

        private void Load()
        {
            money = currentSessionData.Money;

            Items = currentSessionData.MainInventory.GetItemList();
        } 

        public ItemData GetItem(InventoryItem item)
        {
            foreach (var itemData in Items)
            {
                if (itemData.Item == item)
                    return itemData;
            }

            return null;
        }

        /// <summary>
        /// Give an item to inventory
        /// </summary>
        /// <param name="item">item to give</param>
        /// <param name="amount">amount</param>
        public void AddItem(InventoryItem item, int amount, bool showNotify = true)
        {
            ItemData itemData = GetItem(item);
            if (itemData == null)
            {
                ItemData data = new ItemData(item, amount);
                Items.Add(data);
                currentSessionData.MainInventory.AddItem(data);
                if(showNotify)
                {
                    if (amount > 0)
                    {
                        NotificationManager.NewNotification(item.LowSizeIcon, $"{item.ItemName} <color={NotificationManager.GreenColor}>+{amount}</color>", true, item.ItemTextColor, NotificationStyle.Positive);
                    }
                }
            }
            else
            {
                if(showNotify)
                {
                    if (amount > 0)
                    {
                        if (itemData.Amount == 0)
                        {
                            NotificationManager.NewNotification(item.LowSizeIcon, $"{item.ItemName} <color={NotificationManager.GreenColor}>+{amount}</color>", true, item.ItemTextColor, NotificationStyle.Positive);
                        }
                        else
                        {
                            NotificationManager.NewNotification(item.LowSizeIcon, $"{item.ItemName} <color={NotificationManager.GreenColor}>+{amount}</color>", false, item.ItemTextColor, NotificationStyle.Positive);
                        }
                    }
                }
                itemData.Amount += amount;
                currentSessionData.MainInventory.SetItem(itemData);
            }

            LogUtility.WriteLog($"Player got {amount} {item.ItemName}");

            UpdateVisual();
        }

        /// <summary>
        /// Take item from inventory
        /// </summary>
        /// <param name="item">Item to take</param>
        /// <param name="amount">Amount</param>
        public bool TakeItem(InventoryItem item, int amount, bool showNotify = true)
        {
            ItemData itemData = GetItem(item);
            if (itemData == null)
            {
                ItemData data = new ItemData(item);
                Items.Add(data);
                return false;
            }
            else
            {
                if(itemData.Amount >= amount)
                {
                    itemData.Amount -= amount;
                    currentSessionData.MainInventory.SetItem(itemData);

                    if(showNotify)
                    {
                        NotificationManager.NewNotification(item.LowSizeIcon, $"{item.ItemName} <color={NotificationManager.RedColor}>-{amount}</color>", false, item.ItemTextColor, NotificationStyle.Positive);
                    }

                    LogUtility.WriteLog($"Player lost {amount} {item.ItemName}");

                    UpdateVisual();

                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Take items from inventory
        /// </summary>
        /// <param name="items">List of items for take</param>
        public void TakeItem(List<ItemData> items, bool showNotify = true)
        {
            foreach (var item in items)
            {
                TakeItem(item.Item, item.Amount, showNotify);
            }
        }

        public bool CanTakeItems(List<ItemData> items)
        {
            foreach (var item in items)
            {
                ItemData data = GetItem(item.Item);
                if(data != null)
                {
                    if (GetItem(item.Item).Amount < item.Amount)
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        #region Visual
        private void UpdateVisual()
        {
            if (!IsActive)
                return;

            // observers data
            foreach (var observer in observers)
            {
                observer.UpdateData(this);
            }
        }

        public void InventoryEnabled()
        {
            if (!IsActive)
                return;

            InventoryWindow.Open();
        }
        #endregion

        #region IInventoryObserver
        public void Attach(IInventoryObserver observer, bool initialize = false)
        {
            observers.Add(observer);
            if(initialize)
                observer.UpdateData(this);
        }
        public void Detach(IInventoryObserver observer)
        {
            observers.Remove(observer);
        }
        #endregion
    }
}
