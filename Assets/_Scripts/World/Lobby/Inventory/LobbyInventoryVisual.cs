using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AYellowpaper.SerializedCollections;

namespace Game.Lobby.Inventory.Visual
{
    using Game.Inventory;
    using Input;
    using System.Data.Common;

    // TO DO
    public class LobbyInventoryVisual : MonoBehaviour, IUIPanelManagerObserver
    {
        [SerializeField] private GameObject MainPanel;
        [Space]
        [SerializeField] private TextMeshProUGUI FreeSpaceText;
        [Space]
        [SerializeField] private LobbyInventoryTakenItemVisual TakenItemVisualPrefab;
        [SerializeField] private Transform TakenItemVisualsParent;

        private LobbyInventory _inventory;
        private UIPanelManager _uiPanelManager;
        private UIInputHandler _input => InputManager.UIInputHandler;
        private Dictionary<InventoryItem, LobbyInventoryTakenItemVisual> _takenItemVisuals;

        private LobbyInventoryChest _chest;

        public void Initialize(LobbyInventory inventory, Dictionary<InventoryItem, int> items)
        {
            _uiPanelManager = ServiceLocator.GetService<UIPanelManager>();
            _inventory = inventory;
            _takenItemVisuals = new Dictionary<InventoryItem, LobbyInventoryTakenItemVisual>();

            foreach (var item in items.Keys)
            {
                AddTakenItemVisual(new ItemData(item, items[item]));
            }
            UpdateFreeSpaceText();

            _inventory.OnItemAdded += OnItemAdded;
            _inventory.OnItemTaken += OnItemTaken;

            _uiPanelManager.Attach(this);

            _input.CloseEvent += Close;
        }
        private void OnDisable()
        {
            _inventory.OnItemAdded -= OnItemAdded;
            _inventory.OnItemTaken -= OnItemTaken;

            _input.CloseEvent -= Close;
        }

        private void OnItemAdded(ItemData itemData)
        {
            AddTakenItemVisual(itemData);
            UpdateFreeSpaceText();
        }
        private void OnItemTaken(ItemData itemData)
        {
            UpdateTakenItemVisual(_takenItemVisuals[itemData.Item]);
            UpdateFreeSpaceText();
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
        private void UpdateFreeSpaceText()
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

        public void Open(LobbyInventoryChest chest)
        {
            _chest = chest;

            UpdateTakenItemVisual();

            _uiPanelManager.OpenPanel(MainPanel);

            _chest.SetVisualOpened(true);
        }
        private void Close()
        {
            if (!MainPanel.activeSelf)
                return;

            _uiPanelManager.ClosePanel(MainPanel);
            _inventory.ApplyItemsToMainInventory();

            _chest.SetVisualOpened(false);
        } 

        public void PanelStateIsChanged(GameObject panel)
        {
            if (panel != MainPanel && MainPanel.activeSelf)
                MainPanel.SetActive(false);
        }
    }
}
