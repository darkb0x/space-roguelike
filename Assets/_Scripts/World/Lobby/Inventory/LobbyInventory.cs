using UnityEngine;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;

namespace Game.Lobby.Inventory
{
    using Visual;
    using Game.Inventory;
    using System.Linq;

    public class LobbyInventory : Inventory, IService, IEntryComponent
    {
        [SerializeField] public LobbyInventoryVisual Visual;
        [Space]
        [SerializeField, Min(1)] private int m_MaxTakenItemsAmount;
        [Space]
        [SerializeField] private SerializedDictionary<InventoryItem, int> ItemsForSession;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField, ReadOnly] private int m_MaxItemCount;
        [SerializeField, ReadOnly] private int m_FreeItemsCount;
        [SerializeField, ReadOnly] private int m_CurrentItemsAmount;
#endif

        public int MaxTakenItemsAmount => m_MaxTakenItemsAmount;
        public int FreeItemsAmount
        {
            get => MaxTakenItemsAmount - CurrentItemsAmount;
        }
        public int CurrentItemsAmount
        {
            get
            {
                if (ItemsForSession == null && ItemsForSession.Count == 0)
                    return 0;

                int value = 0;
                ItemsForSession.Keys.ToList().ForEach(x => value += ItemsForSession[x]);

                return value;
            }
        }

#if UNITY_EDITOR
        private void UpdateEditorDebugValues()
        {
            m_MaxItemCount = MaxTakenItemsAmount;
            m_FreeItemsCount = FreeItemsAmount;
            m_CurrentItemsAmount = CurrentItemsAmount;
        }
#endif

        public void Initialize()
        {
            Load();

            ItemsForSession = new SerializedDictionary<InventoryItem, int>();

            var itemsInInventory = _currentSessionData.MainInventory.GetItemList();
            if(itemsInInventory.Count > 0)
            {
                itemsInInventory.ForEach(item => AddItem(item, false));
                _currentSessionData.MainInventory.Clear();
            }
            
            Visual.Initialize(this, new Dictionary<InventoryItem, int>(Items));

#if UNITY_EDITOR
            UpdateEditorDebugValues();
#endif
        }
        protected override void Load()
        {
            Money = _currentSessionData.Money;

            Items = new SerializedDictionary<InventoryItem, int>();
            foreach (var item in _currentSessionData.LobbyInventory.GetItemList())
            {
                AddItem(item, false);
            }
        }

        public override void AddItem(ItemData item, bool showNotify = true)
        {
            base.AddItem(item, showNotify);
            _currentSessionData.LobbyInventory.UpdateItemData(GetItem(item.Item));
        }
        public override bool TakeItem(ItemData item, bool showNotify = true)
        {
            if(base.TakeItem(item, showNotify))
            {
                _currentSessionData.LobbyInventory.UpdateItemData(GetItem(item.Item));
                return true;
            }
            return false;
        }

        public bool TryPickItemIntoSession(InventoryItem item, int value)
        {
            if (FreeItemsAmount == 0 && value > 0)
                return false;
            if (CurrentItemsAmount + value > MaxTakenItemsAmount ||
                CurrentItemsAmount + value < 0)
                return false;

            PickItemIntoSession(item, value);

            return true;
        }
        public void PickItemIntoSession(InventoryItem item, int value)
        {
            if(!ItemsForSession.ContainsKey(item))
            {
                ItemsForSession.Add(item, value);
                return;
            }

            ItemsForSession[item] += value;
            Visual.UpdateFreeSpaceText();

#if UNITY_EDITOR
            UpdateEditorDebugValues();
#endif
        }

        public void ApplyItemsToMainInventory()
        {
            ItemsForSession.Keys.ToList().ForEach(
                item =>
                {
                    if (ItemsForSession[item] > 0)
                        _currentSessionData.MainInventory.UpdateItemData(new ItemData(item, ItemsForSession[item]));
                }
            );
        }
    }
}
