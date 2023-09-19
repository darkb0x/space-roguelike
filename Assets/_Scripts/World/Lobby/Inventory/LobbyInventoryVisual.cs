using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Game.Lobby.Inventory.Visual
{
    using Game.Inventory;
    using Input;
    using UI;

    // TO DO
    public class LobbyInventoryVisual : Window
    {
        [SerializeField] private TextMeshProUGUI FreeSpaceText;
        [Space]
        [SerializeField] private LobbyInventoryTakenItemVisual TakenItemVisualPrefab;
        [SerializeField] private Transform TakenItemVisualsParent;

        public override WindowID ID => LobbyInventory.LOBBY_INVENTORY_WINDOW_ID;

        private LobbyInventory _inventory;
        private UIInputHandler _input => InputManager.UIInputHandler;
        private Dictionary<InventoryItem, LobbyInventoryTakenItemVisual> _takenItemVisuals;

        public void Initialize(LobbyInventory inventory, Dictionary<InventoryItem, int> items)
        {
            _inventory = inventory;
            _takenItemVisuals = new Dictionary<InventoryItem, LobbyInventoryTakenItemVisual>();

            foreach (var item in items.Keys)
            {
                AddTakenItemVisual(new ItemData(item, items[item]));
            }
            UpdateFreeSpaceText();

            _inventory.OnItemAdded += OnItemAdded;
            _inventory.OnItemTaken += OnItemTaken;
        }

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();

            _input.CloseEvent += () => Close();
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();

            _inventory.OnItemAdded -= OnItemAdded;
            _inventory.OnItemTaken -= OnItemTaken;

            _input.CloseEvent -= () => Close();
        }

        private void OnItemAdded(ItemData itemData)
        {
            AddTakenItemVisual(itemData);
            UpdateFreeSpaceText();
        }
        private void OnItemTaken(ItemData itemData)
        {
            if (_takenItemVisuals.ContainsKey(itemData.Item))
            {
                UpdateTakenItemVisual(_takenItemVisuals[itemData.Item]);
                UpdateFreeSpaceText();
            }
        }

        private void AddTakenItemVisual(ItemData itemData)
        {
            var item = itemData.Item;

            if(_takenItemVisuals.ContainsKey(item))
            {
                UpdateTakenItemVisual(_takenItemVisuals[item]);
                return;
            }

            if(item.CanTakeInMission)
            {
                var takenItemVisual = Instantiate(TakenItemVisualPrefab, TakenItemVisualsParent);
                takenItemVisual.Initialize(item);
                takenItemVisual.ItemSlider.onValueChanged.AddListener(_ => UpdateFreeSpaceText());
                _takenItemVisuals.Add(item, takenItemVisual);
            }
        }
        public void UpdateFreeSpaceText()
        {
            FreeSpaceText.text = $"({_inventory.CurrentItemsAmount}/{_inventory.MaxTakenItemsAmount})";
        }
        private void UpdateTakenItemVisual()
        {
            foreach (var item in _takenItemVisuals.Keys)
            {
                _takenItemVisuals[item].UpdateData();
            }
        }
        private void UpdateTakenItemVisual(LobbyInventoryTakenItemVisual visual)
        {
            visual.UpdateData();
        }

        public override void Open(bool notify = true)
        {
            UpdateTakenItemVisual();
            base.Open(notify);
        }

        public override void Close(bool notify = true)
        {
            base.Close(notify);
            _inventory?.ApplyItemsToMainInventory();
        }

    }
}
