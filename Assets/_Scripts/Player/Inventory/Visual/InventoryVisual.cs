using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Inventory.Visual
{
    public class InventoryVisual : MonoBehaviour, IInventoryObserver
    {
        [SerializeField] private InventoryVisualItem ItemVisualPrefab;
        [SerializeField] private Transform ItemVisualsParent;

        public bool isOpened { get; private set; }
        private List<InventoryVisualItem> itemVisuals = new List<InventoryVisualItem>();

        private void Start()
        {
            ServiceLocator.GetService<PlayerInventory>().Attach(this, true);
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

        public void UpdateData(PlayerInventory inventory)
        {
            foreach (var itemData in inventory.Items)
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
