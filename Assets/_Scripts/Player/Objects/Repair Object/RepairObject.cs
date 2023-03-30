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

        private List<RepairObjectItemUIVisual> ItemsForRepairInUI;
        private PlayerInteractObject playerInteract;
        public bool isRepaired { get; private set; }

        private void Start()
        {
            ItemsForRepairInUI = new List<RepairObjectItemUIVisual>();

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
            if (isRepaired)
                return;

            foreach (var item in ItemsForRepairInUI)
            {
                item.UpdateVisual(item.itemData, GetItemAmount(item.itemData), GetItemColor(item.itemData));
            }

            ItemsVisualParent.gameObject.SetActive(true);
        }
        private void PlayerExit(Collider2D coll)
        {
            if (isRepaired)
                return;

            ItemsVisualParent.gameObject.SetActive(false);
        }

        public void Repair()
        {
            if (isRepaired)
                return;
            if (!PlayerInventory.Instance.CanTakeItems(ItemsForRepair))
                return;

            PlayerInventory.Instance.TakeItem(ItemsForRepair);

            AfterRepair.Invoke();
            isRepaired = true;

            ItemsVisualParent.gameObject.SetActive(false);

            playerInteract.OnPlayerEnter -= PlayerEnter;
            playerInteract.OnPlayerExit -= PlayerExit;
        }

        private Color GetItemColor(InventoryItem item)
        {
            ItemData repairItem = default;
            foreach (var itemForRepair in ItemsForRepair)
            {
                if (itemForRepair.Item == item)
                    repairItem = itemForRepair;
            }

            if(PlayerInventory.Instance.GetItem(repairItem.Item) != null)
            {
                if (PlayerInventory.Instance.GetItem(repairItem.Item).Amount < repairItem.Amount)
                {
                    return Color.red;
                }
                else
                {
                    return Color.white;
                }
            }
            else
            {
                return Color.red;
            }
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
