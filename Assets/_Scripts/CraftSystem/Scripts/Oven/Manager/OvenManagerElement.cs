using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.CraftSystem.Oven.Manager
{
    using Game.Inventory;

    public class OvenManagerElement : MonoBehaviour
    {
        OvenManager manager;

        [SerializeField] private OvenConfig.craft currentCraft;

        [Header("UI")]
        [SerializeField] private Transform firstItems_parent;
        [SerializeField] private Transform finalItem_parent;
        [Space]
        [SerializeField] private GameObject itemPrefab;

        private List<TextMeshProUGUI> amountsText = new List<TextMeshProUGUI>();

        private IInventory PlayerInventory;

        public void Initialize(OvenConfig.craft craft, OvenManager m)
        {
            PlayerInventory = ServiceLocator.GetService<IInventory>();

            currentCraft = craft;
            manager = m;

            // first items
            for (int i = 0; i < currentCraft.firstItems.Count; i++)
            {
                GameObject obj = Instantiate(itemPrefab, firstItems_parent);

                obj.GetComponentInChildren<Image>().sprite = currentCraft.firstItems[i].Item.Icon;

                TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
                text.text = currentCraft.firstItems[i].Amount.ToString();

                amountsText.Add(text);
            }

            // final item
            GameObject obj_final = Instantiate(itemPrefab, finalItem_parent);

            obj_final.GetComponentInChildren<Image>().sprite = currentCraft.finalItem.Item.Icon;
            obj_final.GetComponentInChildren<TextMeshProUGUI>().text = currentCraft.finalItem.Amount.ToString();
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
                ItemData item = currentCraft.firstItems[i];
                ItemData data = PlayerInventory.GetItem(item.Item);
                if(data != null)
                {
                    amountsText[i].color = data.Amount >= item.Amount ? Color.white : Color.red;
                }
                else
                {
                    amountsText[i].color = Color.red;
                }
            }
        }
    }
}
