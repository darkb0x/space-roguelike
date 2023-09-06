using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    public class InventoryVisual : MonoBehaviour
    {
        [SerializeField] private InventoryVisualItem ItemVisualPrefab;
        [SerializeField] private Transform ItemVisualsParent;

        public bool isOpened { get; private set; }
        private List<InventoryVisualItem> itemVisuals = new List<InventoryVisualItem>();

        private Inventory _inventory;

        private void Start()
        {
            _inventory = ServiceLocator.GetService<Inventory>();

            SubscribeToEvents();
        }
        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            _inventory.OnItemAdded += x => UpdateData();
            _inventory.OnItemTaken += x => UpdateData();
        }
        private void UnsubscribeFromEvents()
        {
            _inventory.OnItemAdded -= x => UpdateData();
            _inventory.OnItemTaken -= x => UpdateData();
        }

        #region Item Visuals
        public InventoryVisualItem AddItemVisual(InventoryItem item, int amount = 0)
        {
            InventoryVisualItem visual = Instantiate(ItemVisualPrefab.gameObject, ItemVisualsParent).GetComponent<InventoryVisualItem>();
            visual.UpdateVisual(item, amount);

            itemVisuals.Add(visual);

            return visual;
        }

        private InventoryVisualItem GetItemVisual(InventoryItem item)
        {
            foreach (var visual in itemVisuals)
            {
                if (visual.Item == item)
                    return visual;
            }
            return null;
        }
        #endregion

        public void UpdateData()
        {
            foreach (var itemData in _inventory.GetItems())
            {
                InventoryVisualItem visual = GetItemVisual(itemData.Item);

                if (visual == null)
                {
                    AddItemVisual(itemData.Item, itemData.Amount);
                    continue;
                }

                visual.UpdateVisual(itemData.Item, itemData.Amount);

            }
        }
    }
}
