using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Player
{
    using Inventory;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class RepairObject : MonoBehaviour
    {
        [SerializeField] private List<ItemData> ItemsForRepair = new List<ItemData>();
        [Space]
        [SerializeField] private RepairObjectItemUIVisual ItemForRepairVisual;
        [SerializeField] private Transform ItemsVisualParent;
        [Space]
        [SerializeField] private UnityEvent AfterRepair;

        private List<RepairObjectItemUIVisual> ItemsForRepairInUI = new List<RepairObjectItemUIVisual>();
        private PlayerInteractObject playerInteract;

        private void Start()
        {
            foreach (var item in ItemsForRepair)
            {
                RepairObjectItemUIVisual visual = Instantiate(ItemForRepairVisual.gameObject, ItemsVisualParent).GetComponent<RepairObjectItemUIVisual>();
                visual.UpdateVisual(item.Item, item.Amount, GetItemColor(item.Item));
                ItemsForRepairInUI.Add(visual);
            }
            ItemsVisualParent.gameObject.SetActive(false);

            playerInteract = GetComponent<PlayerInteractObject>();
            playerInteract.OnPlayerEnter += PlayerEnter;
            playerInteract.OnPlayerExit += PlayerExit;
        }

        private void PlayerEnter(Collider2D coll)
        {
            foreach (var item in ItemsForRepairInUI)
            {
                item.UpdateVisual(item.itemData, GetItemAmount(item.itemData), GetItemColor(item.itemData));
            }

            ItemsVisualParent.gameObject.SetActive(true);
        }
        private void PlayerExit(Collider2D coll)
        {
            ItemsVisualParent.gameObject.SetActive(false);
        }

        public void Repair()
        {
            if (!CanTakeItems())
                return;

            TakeItems();

            AfterRepair.Invoke();

            playerInteract.OnPlayerEnter -= PlayerEnter;
            playerInteract.OnPlayerExit -= PlayerExit;
        }

        private bool CanTakeItems()
        {
            foreach (var item in ItemsForRepair)
            {
                if(PlayerInventory.instance.GetItem(item.Item).Amount < item.Amount)
                {
                    return false;
                }
            }
            return true;
        }
        private void TakeItems()
        {
            foreach (var item in ItemsForRepair)
            {
                PlayerInventory.instance.TakeItem(item.Item, item.Amount);
            }
        }

        private Color GetItemColor(InventoryItem item)
        {
            ItemData repairItem = default;
            foreach (var itemForRepair in ItemsForRepair)
            {
                if (itemForRepair.Item == item)
                    repairItem = itemForRepair;
            }

            if (PlayerInventory.instance.GetItem(repairItem.Item).Amount < repairItem.Amount)
                return Color.red;
            else
                return Color.white;
        }
        private int GetItemAmount(InventoryItem item)
        {
            foreach (var itemForRepair in ItemsForRepair)
            {
                if (itemForRepair.Item == item)
                    return itemForRepair.Amount;
            }
            return -1;
        }
    }
}
