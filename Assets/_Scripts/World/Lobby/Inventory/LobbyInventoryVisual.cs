using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Game.Lobby.Inventory.Visual
{
    using Player.Inventory.Visual;
    using Player.Inventory;

    public class LobbyInventoryVisual : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject MainPanel;

        [Header("Default inventory")]
        [SerializeField] private Transform InventoryVisualParent;
        [SerializeField] private InventoryVisualItem InventoryItemVisual;

        [Header("Taken items inventory")]
        [SerializeField] private TextMeshProUGUI FreeSpaceText;
        [Space]
        [SerializeField] private Transform TakenItemsVisualParent;
        [SerializeField] private LobbyInventoryTakenItemVisual TakenItemVisual;

        [HideInInspector] public List<InventoryVisualItem> InventoryItemVisuals = new List<InventoryVisualItem>();
        [HideInInspector] public List<LobbyInventoryTakenItemVisual> TakenItemVisuals = new List<LobbyInventoryTakenItemVisual>();
        private LobbyInventory inventory;
        public bool isOpened { get; private set; }
        private Animator currentChestAnimator;

        private void Start()
        {
            GameInput.InputActions.UI.CloseWindow.performed += ClosePanel;
        }

        public void Initialize(List<ItemData> datas, LobbyInventory lobbyInventory)
        {
            inventory = lobbyInventory;

            foreach (var itemData in datas)
            {
                AddInventoryItemVisual(itemData);
                AddTakenItemVisual(itemData, lobbyInventory);
            }
        }

        public void OpenPanel(Animator chestAnim)
        {
            currentChestAnimator = chestAnim;
            currentChestAnimator.SetBool("isOpened", true);

            UIPanelManager.Instance.OpenPanel(MainPanel);
            isOpened = true;
        }
        public void ClosePanel()
        {
            if(currentChestAnimator != null)
            {
                currentChestAnimator.SetBool("isOpened", false);
                currentChestAnimator = null;
            }

            UIPanelManager.Instance.ClosePanel(MainPanel);
            inventory.SetItemsToInventory();
            isOpened = true;
        }
        public void ClosePanel(InputAction.CallbackContext callback)
        {
            ClosePanel();
        }

        public void UpdateFreeSpaceText(int currentAmount, int maxAmount)
        {
            FreeSpaceText.text = $"({currentAmount}/{maxAmount})";
        }

        public void UpdateItemsInInventory(List<ItemData> itemDatas)
        {
            foreach (var itemData in itemDatas)
            {
                InventoryVisualItem visual = GetItemVisual(itemData.Item);

                if (visual == null)
                {
                    AddInventoryItemVisual(itemData);
                    continue;
                }

                visual.UpdateVisual(itemData.Item, itemData.Amount);

            }
        }
        public void UpdateTakenItems()
        {
            foreach (var visual in TakenItemVisuals)
            {
                ItemData data = GetDataByItem(visual.ItemData.Item);
                visual.UpdateData();
            }
        }

        #region Utilities
        private void AddInventoryItemVisual(ItemData data)
        {
            InventoryVisualItem inventoryItem = Instantiate(InventoryItemVisual.gameObject, InventoryVisualParent).GetComponent<InventoryVisualItem>();
            inventoryItem.UpdateVisual(data.Item, data.Amount);

            InventoryItemVisuals.Add(inventoryItem);
        }
        private void AddTakenItemVisual(ItemData data, LobbyInventory lobbyInventory)
        {
            LobbyInventoryTakenItemVisual takenItem = Instantiate(TakenItemVisual.gameObject, TakenItemsVisualParent).GetComponent<LobbyInventoryTakenItemVisual>();
            takenItem.Initialize(data, lobbyInventory);

            TakenItemVisuals.Add(takenItem);
        }

        private InventoryVisualItem GetItemVisual(InventoryItem item)
        {
            foreach (var visual in InventoryItemVisuals)
            {
                if (visual.Item == item)
                    return visual;
            }
            return null;
        }
        private ItemData GetDataByItem(InventoryItem item)
        {
            foreach (var data in inventory.LobbyItems)
            {
                if (data.Item == item)
                {
                    return data;
                }
            }
            return null;
        }
        #endregion
    }
}
