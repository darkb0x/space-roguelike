using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.CraftSystem.Oven.Manager
{
    using Player.Inventory;
    using Input;

    public class OvenManager : MonoBehaviour, IUIPanelManagerObserver
    {
        [SerializeField, Expandable] private OvenConfig craftList;

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
        private UIInputHandler _input => InputManager.UIInputHandler;

        private void Start()
        {
            UIPanelManager = Singleton.Get<UIPanelManager>();
            PlayerInventory = Singleton.Get<PlayerInventory>();

            UIPanelManager.Attach(this);

            foreach (var item in craftList.Items)
            {
                OvenManagerElement element = Instantiate(craftElement.gameObject, craftListParent).GetComponent<OvenManagerElement>();
                element.Initialize(item, this);

                craftsVisuals.Add(element);
            } 
            
            _input.CloseEvent += ClosePanel;
        }
        private void OnDisable()
        {
            _input.CloseEvent -= ClosePanel;
        }

        public void RemeltingItem(OvenConfig.craft craft)
        {
            List<ItemData> itemsData = ConvertOvenCraftToItemData(craft);

            if (PlayerInventory.CanTakeItems(itemsData))
            {
                PlayerInventory.TakeItem(itemsData);
                currentOven.StartRemelting(craft);
                ClosePanel();
            }
        }

        private List<ItemData> ConvertOvenCraftToItemData(OvenConfig.craft craft)
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
