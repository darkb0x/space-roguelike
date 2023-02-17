using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Oven.Manager
{
    using Player.Inventory;

    public class OvenManagerElement : MonoBehaviour, IInventoryObserver
    {
        OvenManager manager;

        [SerializeField] private OvenCraftList.craft currentCraft;

        [Header("UI")]
        [SerializeField] private Transform firstItems_parent;
        [SerializeField] private Transform finalItem_parent;
        [Space]
        [SerializeField] private GameObject itemPrefab;

        List<TextMeshProUGUI> amountsText = new List<TextMeshProUGUI>();

        public void Initialize(OvenCraftList.craft craft, OvenManager m)
        {
            PlayerInventory.instance.observers.Add(this);

            currentCraft = craft;
            manager = m;

            // first items
            for (int i = 0; i < currentCraft.firstItems.Count; i++)
            {
                GameObject obj = Instantiate(itemPrefab, firstItems_parent);

                obj.GetComponentInChildren<Image>().sprite = currentCraft.firstItems[i].item.Icon;

                TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
                text.text = currentCraft.firstItems[i].amount.ToString();

                amountsText.Add(text);
            }

            // final item
            GameObject obj_final = Instantiate(itemPrefab, finalItem_parent);

            obj_final.GetComponentInChildren<Image>().sprite = currentCraft.finalItem.item.Icon;
            obj_final.GetComponentInChildren<TextMeshProUGUI>().text = currentCraft.finalItem.amount.ToString();
        }

        public void StartRemelting()
        {
            manager.RemeltingItem(currentCraft);
        }

        // interface
        public void UpdateData(PlayerInventory inventory)
        {
            if (!gameObject.activeInHierarchy)
                return;

            for (int i = 0; i < amountsText.Count; i++)
            {
                OvenCraftList.craft.s_item item = currentCraft.firstItems[i];
                amountsText[i].color = inventory.GetItem(item.item).amount >= item.amount ? Color.white : Color.red;
            }
        }
    }
}
