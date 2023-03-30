using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace Game.Player.Inventory
{
    using SaveData;
    using Visual;

    public interface IInventoryObserver
    {
        public void UpdateData(PlayerInventory inventory);
    }

    [System.Serializable]
    public class ItemData
    {
        [Expandable] public InventoryItem Item;
        public int Amount;

        public ItemData(InventoryItem item, int amount = 0)
        {
            Item = item;
            Amount = amount;
        }
    }

    public class PlayerInventory : MonoBehaviour
    {
        // singletone
        public static PlayerInventory Instance;
        private void Awake() => Instance = this;

        [Header("Inventory")]
        [SerializeField] private bool AllItemsAtStart = false;
        [SerializeField, ShowIf("AllItemsAtStart")] private InventoryItem[] AllItems;
        [Space]
        [ReadOnly] public List<ItemData> Items = new List<ItemData>();
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

        [Header("Visual")]
        [SerializeField] private InventoryNotificationsVisual NotificationsVisual;
        [Space]
        [SerializeField] private TextMeshProUGUI AllItemsAmountText;
        [Space]
        [SerializeField] private Animator InventoryAnim;
        [SerializeField, AnimatorParam("InventoryAnim")] private string InventoryAnim_OpenBool;

        private bool isOpened = false;
        public List<IInventoryObserver> observers = new List<IInventoryObserver>();
        private SessionData currentSessionData => GameData.Instance.CurrentSessionData;
        private GameObject inventoryVisualGameObj;

        private void Start()
        {
            inventoryVisualGameObj = InventoryAnim.gameObject;

            GameInput.InputActions.Player.Inventory.performed += InventoryEnabled;

            Load();

            UpdateVisual();
        }

        private void Update()
        {
            if(inventoryVisualGameObj.activeInHierarchy)
            {
                InventoryAnim.SetBool(InventoryAnim_OpenBool, isOpened);
            }

            if(Keyboard.current.hKey.isPressed)
            {
                foreach (var item in AllItems)
                {
                    ItemData data = new ItemData(item, 100);
                    Items.Add(data);
                    currentSessionData.MainInventory.AddItem(data);
                }
                currentSessionData.Save();
            }
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
                Items.Add(new ItemData(item, amount));
                if(showNotify)
                {
                    if (amount > 0)
                    {
                        NotificationsVisual.NewInventoryNotification(item, amount, true, false);
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
                            NotificationsVisual.NewInventoryNotification(item, amount, true, false);
                        }
                        else
                        {
                            NotificationsVisual.NewInventoryNotification(item, amount, false, false);
                        }
                    }
                }
                itemData.Amount += amount;
            }

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
                Items.Add(new ItemData(item));
                return false;
            }
            else
            {
                if(itemData.Amount >= amount)
                {
                    itemData.Amount -= amount;

                    if(showNotify)
                    {
                        NotificationsVisual.NewInventoryNotification(item, amount, false, true);
                    }

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
        public void TakeItem(List<ItemData> items)
        {
            foreach (var item in items)
            {
                TakeItem(item.Item, item.Amount);
            }
        }

        public bool CanTakeItems(List<ItemData> items)
        {
            foreach (var item in items)
            {
                if (GetItem(item.Item).Amount < item.Amount)
                    return false;
            }
            return true;
        }

        private void OnDisable()
        {
            GameInput.InputActions.Player.Inventory.performed -= InventoryEnabled;
        }

        #region Visual
        private void UpdateVisual()
        {
            int allItemsAmount = 0;

            // main
            foreach (var itemData in Items)
            {
                allItemsAmount += itemData.Amount;
            }
            AllItemsAmountText.text = allItemsAmount.ToString();

            // observers data
            foreach (var observer in observers)
            {
                observer.UpdateData(this);
            }
        }

        public void InventoryEnabled(UnityEngine.InputSystem.InputAction.CallbackContext callback)
        {
            isOpened = !isOpened;
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
