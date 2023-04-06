using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Lobby.Inventory
{
    using Visual;
    using Player.Inventory;
    using SaveData;

    public class LobbyInventory : MonoBehaviour
    {
        public static LobbyInventory Instance;

        [SerializeField] private LobbyInventoryVisual Visual;

        [Space]
        public int MaxTakenItemsAmount = 20;

        public List<ItemData> LobbyItems = new List<ItemData>();
        public Dictionary<InventoryItem, int> Items = new Dictionary<InventoryItem, int>();
        public int FreeItemsAmount { get; private set; }

        private SessionData currentSessionData => GameData.Instance.CurrentSessionData;
        public System.Action<ItemData> OnNewItem;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            FreeItemsAmount = MaxTakenItemsAmount;

            LobbyItems = currentSessionData.LobbyInventory.GetItemList();

            foreach (var item in currentSessionData.MainInventory.GetItemList())
            {
                AddItem(item, false);
            }

            currentSessionData.Save();

            Visual.Initialize(LobbyItems, this);
        }

        private void Update()
        {
            UpdateFreeSpace();
        }

        public void SetItemsToInventory()
        {
            currentSessionData.MainInventory.Clear();
            foreach (var item in Items.Keys)
            {
                ItemData itemData = new ItemData(item, Items[item]);
                currentSessionData.MainInventory.AddItem(itemData);
            }
            currentSessionData.Save();
        }

        public void AddItem(ItemData data, bool updateVisual = true)
        {
            ItemData itemData = GetItem(data.Item);
            if(itemData != null)
            {
                itemData.Amount += data.Amount;
                currentSessionData.LobbyInventory.SetItem(itemData);
                currentSessionData.Save();

                if (updateVisual)
                {
                    Visual.UpdateItemsInInventory(LobbyItems);
                }
            }
            else
            {
                itemData = new ItemData(data.Item, data.Amount);
                LobbyItems.Add(itemData);
                currentSessionData.LobbyInventory.AddItem(itemData);
                currentSessionData.Save();

                if (updateVisual)
                {
                    Visual.UpdateItemsInInventory(LobbyItems);
                    Visual.UpdateTakenItems(LobbyItems);
                }

                OnNewItem?.Invoke(itemData);
            }
        }
        public bool TakeItem(InventoryItem item, int amount)
        {
            ItemData itemData = GetItem(item);
            if (itemData != null)
            {
                if(itemData.Amount >= amount)
                {
                    itemData.Amount -= amount;
                    currentSessionData.LobbyInventory.SetItem(itemData);
                    Visual.UpdateItemsInInventory(LobbyItems);

                    currentSessionData.Save();
                    return true;
                }
                return false;
            }
            return false;
        }

        public ItemData GetItem(InventoryItem item)
        {
            foreach (var itemData in LobbyItems)
            {
                if(itemData.Item == item)
                {
                    return itemData;
                }
            }
            return null;
        }

        public void UpdateTakenItems(ItemData data)
        {
            if(data.Amount > 0)
            {
                if(Items.ContainsKey(data.Item))
                {
                    Items[data.Item] = data.Amount;
                }
                else
                {
                    Items.Add(data.Item, data.Amount);
                }
            }
            else
            {
                if (Items.ContainsKey(data.Item))
                {
                    Items.Remove(data.Item);
                }
            }
        }

        private void UpdateFreeSpace()
        {
            int currentAmount = 0;
            foreach (var item in Visual.TakenItemVisuals)
            {
                currentAmount += item.ItemData.Amount;

                UpdateTakenItems(item.ItemData);
            }

            FreeItemsAmount = MaxTakenItemsAmount - currentAmount;
            Visual.UpdateFreeSpaceText(Mathf.Clamp(currentAmount, 0, MaxTakenItemsAmount), MaxTakenItemsAmount);
        }
    }
}
