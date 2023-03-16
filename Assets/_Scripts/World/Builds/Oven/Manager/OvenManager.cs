using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game.Oven.Manager
{
    using Player.Inventory;

    public class OvenManager : MonoBehaviour
    {
        [SerializeField, Expandable] private OvenCraftList craftList;

        [Header("UI")]
        [SerializeField, Tooltip("Canvas/Oven")] private GameObject panel;
        [Space]
        [SerializeField] private OvenManagerElement craftElement;
        [SerializeField, Tooltip("Canvas/Oven/Craft list/Scroll View/Viewport/Content")] private Transform craftListParent;

        Oven currentOven;
        public bool isOpened { get; private set; }

        private void Start()
        {
            GameInput.InputActions.UI.CloseWindow.performed += ClosePanel;

            foreach (var item in craftList.Items)
            {
                OvenManagerElement element = Instantiate(craftElement.gameObject, craftListParent).GetComponent<OvenManagerElement>();
                element.Initialize(item, this);
            }
        }

        public void RemeltingItem(OvenCraftList.craft craft)
        {
            List<ItemData> itemsData = ConvertOvenCraftToItemData(craft);

            if (PlayerInventory.Instance.CanTakeItems(itemsData))
            {
                PlayerInventory.Instance.TakeItem(itemsData);
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
            currentOven = oven;

            isOpened = true;

            UIPanelManager.Instance.OpenPanel(panel);
        }
        public void ClosePanel()
        {
            isOpened = false;

            UIPanelManager.Instance.ClosePanel(panel);
        }
        public void ClosePanel(InputAction.CallbackContext context)
        {
            ClosePanel();
        }
        #endregion
    }
}
