using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Oven.Manager
{
    using Player.Inventory;

    public class OvenManagerElement : MonoBehaviour
    {
        OvenManager manager;

        [SerializeField] private OvenCraftList.craft currentCraft;

        [Header("UI")]
        [SerializeField] private Transform firstItems_parent;
        [SerializeField] private Transform finalItem_parent;
        [Space]
        [SerializeField] private GameObject itemPrefab;

        private List<TextMeshProUGUI> amountsText = new List<TextMeshProUGUI>();

        private PlayerInventory PlayerInventory;

        public void Initialize(OvenCraftList.craft craft, OvenManager m)
        {
            PlayerInventory = Singleton.Get<PlayerInventory>();

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
        public void UpdateData()
        {
            for (int i = 0; i < amountsText.Count; i++)
            {
                OvenCraftList.craft.s_item item = currentCraft.firstItems[i];
                ItemData data = PlayerInventory.GetItem(item.item);
                if(data != null)
                {
                    amountsText[i].color = data.Amount >= item.amount ? Color.white : Color.red;
                }
                else
                {
                    amountsText[i].color = Color.red;
                }
            }
        }
    }
}
