using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game.Oven.Manager
{
    using Player.Inventory;

    public class OvenManager : MonoBehaviour, IUIPanelManagerObserver
    {
        [SerializeField, Expandable] private OvenCraftList craftList;

        [Header("UI")]
        [SerializeField, Tooltip("Canvas/Oven")] private GameObject panel;
        [Space]
        [SerializeField] private OvenManagerElement craftElement;
        [SerializeField, Tooltip("Canvas/Oven/Craft list/Scroll View/Viewport/Content")] private Transform craftListParent;

        Oven currentOven;
        public bool isOpened { get; private set; }
        private List<OvenManagerElement> craftsVisuals = new List<OvenManagerElement>();

        private UIPanelManager UIPanelManager;
        private PlayerInventory PlayerInventory;

        private void Start()
        {
            UIPanelManager = Singleton.Get<UIPanelManager>();
            PlayerInventory = Singleton.Get<PlayerInventory>();

            GameInput.InputActions.UI.CloseWindow.performed += ClosePanel;
            UIPanelManager.Attach(this);

            foreach (var item in craftList.Items)
            {
                OvenManagerElement element = Instantiate(craftElement.gameObject, craftListParent).GetComponent<OvenManagerElement>();
                element.Initialize(item, this);

                craftsVisuals.Add(element);
            }
        }
        private void OnDisable()
        {
            GameInput.InputActions.UI.CloseWindow.performed -= ClosePanel;
        }

        public void RemeltingItem(OvenCraftList.craft craft)
        {
            List<ItemData> itemsData = ConvertOvenCraftToItemData(craft);

            if (PlayerInventory.CanTakeItems(itemsData))
            {
                PlayerInventory.TakeItem(itemsData);
                currentOven.StartRemelting(craft);
                ClosePanel();
            }
        }

        private List<ItemData> ConvertOvenCraftToItemData(OvenCraftList.craft craft)
        {
            List<ItemData> itemsData = new List<ItemData>();
            foreach (var item in craft.firstItems)
            {
                ItemData data = new ItemData(item.item, item.amount);
                itemsData.Add(data);
            }
            return itemsData;
        }

        #region UI Interaction
        public void OpenPanel(Oven oven)
        {
            foreach (var visual in craftsVisuals)
            {
                visual.UpdateData();
            }

            currentOven = oven;

            isOpened = true;

            UIPanelManager.OpenPanel(panel);
        }
        public void ClosePanel()
        {
            if (!isOpened)
                return;

            isOpened = false;

            UIPanelManager.ClosePanel(panel);
        }
        public void ClosePanel(InputAction.CallbackContext context)
        {
            ClosePanel();
        }
        #endregion

        public void PanelStateIsChanged(GameObject panel)
        {
            if(panel != this.panel)
            {
                if(isOpened)
                {
                    isOpened = false;
                }
            }
        }
    }
}
