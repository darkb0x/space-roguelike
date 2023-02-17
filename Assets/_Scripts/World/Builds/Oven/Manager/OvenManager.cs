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
        [SerializeField] private GameObject panel;
        [Space]
        [SerializeField] private OvenManagerElement craftElement;
        [SerializeField] private Transform craftListParent;

        Oven currentOven;
        bool isOpened = false;

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
            if(CanCraft(craft))
            {
                TakeItemsFromInventory(craft);
                currentOven.StartRemelting(craft);
                ClosePanel();
            }
        }

        private bool CanCraft(OvenCraftList.craft craft)
        {
            foreach (var item in craft.firstItems)
            {
                if (PlayerInventory.instance.GetItem(item.item).amount < item.amount)
                    return false;
            }
            return true;
        }
        private void TakeItemsFromInventory(OvenCraftList.craft craft)
        {
            foreach (var item in craft.firstItems)
            {
                PlayerInventory.instance.TakeItem(item.item, item.amount);
            }
        }

        #region UI Interaction
        public void OpenPanel(Oven oven)
        {
            currentOven = oven;

            isOpened = true;

            UIPanelManager.manager.OpenPanel(panel);
        }
        public void ClosePanel()
        {
            isOpened = false;

            UIPanelManager.manager.ClosePanel(panel);
        }
        public void ClosePanel(InputAction.CallbackContext context)
        {
            ClosePanel();
        }
        #endregion
    }
}
