using Game.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace Game.Inventory
{
    using Notifications;

    public abstract class Inventory : MonoBehaviour, IInventory
    {
        [SerializeField] protected int m_Money;
        [SerializeField] protected SerializedDictionary<InventoryItem, int> Items;

        public int Money { get; protected set; }

        public Action<int> OnMoneyChanged;
        public Action<ItemData> OnItemAdded;
        public Action<ItemData> OnItemTaken;

        protected SessionData _currentSessionData => SaveDataManager.Instance.CurrentSessionData;

        protected abstract void Load();

        public virtual void AddMoney(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            Money += amount;

            OnMoneyChanged?.Invoke(Money);
        }

        public virtual bool TakeMoney(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (Money >= amount)
            {
                Money -= amount;

                OnMoneyChanged?.Invoke(Money);
                return true;
            }

            return false;
        }

        public ItemData GetItem(InventoryItem item)
        {
            if (Items.ContainsKey(item))
                return new ItemData(item, Items[item]);

            return null;
        }
        public List<ItemData> GetItems()
        {
            List<ItemData> items = new();
            foreach (var item in Items.Keys)
            {
                items.Add(GetItem(item));
            }
            return items;
        }

        public virtual void AddItem(ItemData item, bool showNotify = true)
        {
            ItemDataErrorCheck(item);
            bool itemIsNew = false;

            if (Items.ContainsKey(item.Item))
            {
                Items[item.Item] += item.Amount;
                itemIsNew = true;
            }
            else
            {
                Items.Add(item.Item, item.Amount);
            }

            if (showNotify && item.Amount > 0)
            {
                var invItem = item.Item; // invItem = inventory item

                NotificationManager.NewNotification(
                    invItem.Icon,
                    $"{invItem.ItemName} <color={NotificationManager.GreenColor}>+{item.Amount}</color>",
                    itemIsNew,
                    invItem.ItemTextColor,
                    NotificationStyle.Positive
                    );
            }
            OnItemAdded?.Invoke(item);

            LogUtility.WriteLog($"Player got {item.Amount} {item.Item.ItemName}");
        }

        public virtual bool TakeItem(ItemData item, bool showNotify = true)
        {
            ItemDataErrorCheck(item);

            if (Items.ContainsKey(item.Item) &&
                Items[item.Item] >= item.Amount)
            {
                Items[item.Item] -= item.Amount;

                if(showNotify)
                {
                    var invItem = item.Item; // invItem = inventory item

                    NotificationManager.NewNotification(
                        invItem.LowSizeIcon, 
                        $"{invItem.ItemName} <color={NotificationManager.RedColor}>-{item.Amount}</color>",
                        false, 
                        invItem.ItemTextColor, 
                        NotificationStyle.Negative
                        );
                }
                OnItemTaken?.Invoke(item);

                LogUtility.WriteLog($"Player lost {item.Amount} {item.Item.ItemName}");
                return true;
            }

            return false;
        }

        public virtual bool TakeItem(List<ItemData> items, bool showNotify = true)
        {
            if (items.Any(x => CanTakeItem(x)))
                return false;

            foreach (var item in items)
            {
                TakeItem(item, showNotify);
            }

            return true;
        }

        private bool CanTakeItem(ItemData item)
        {
            if (!Items.ContainsKey(item.Item))
                return false;
            if (Items[item.Item] < item.Amount)
                return false;

            return true;
        }
        private void ItemDataErrorCheck(ItemData item)
        {
            if (item == null)
                throw new ArgumentOutOfRangeException(nameof(item));
            if (item.Item == null)
                throw new ArgumentOutOfRangeException(nameof(item.Item));
            if (item.Amount < 0)
                throw new ArgumentOutOfRangeException(nameof(item.Amount));
        }
    }
}
