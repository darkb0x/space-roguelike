using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

namespace Game.CraftSystem
{
    using CraftSystem.Editor.ScriptableObjects;
    using Player.Inventory;

    public class CSCraftUICraft : MonoBehaviour
    {
        private CSManager craftSystem;
        List<TextMeshProUGUI> amountsText = new List<TextMeshProUGUI>();

        [Header("Variables")]
        [ReadOnly, Expandable] public CSCraftSO craft;
        [Space]
        [SerializeField] private Animator anim;
        [SerializeField, AnimatorParam("anim")] private string animValue_enabled;

        [Header("UI")]
        [SerializeField] private Image craftButtonImage;
        [SerializeField] private Button craftButton;
        [Space]
        [SerializeField] private TextMeshProUGUI craft_name;
        [SerializeField] private Image craft_icon;
        [Space]
        [SerializeField] private Transform itemListTransform;
        [SerializeField] private GameObject itemListComponent;

        private PlayerInventory PlayerInventory;

        public void Initialize(CSCraftSO craftSO, CSManager manager)
        {
            PlayerInventory = Singleton.Get<PlayerInventory>();
            craftSystem = manager;

            // Variables
            craft = craftSO;

            //UI
            craft_name.text = craft.CraftName;
            craft_icon.sprite = craft.IconSprite;

            foreach (var item in craft.ObjectCraft)
            {
                if (item == null | item.Item == null)
                {
                    Debug.Log(craft.AssetPath + " Object craft have null item, please fix it!");
                    continue;
                }

                GameObject obj = Instantiate(itemListComponent, itemListTransform);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = item.Item.Icon; // item icon

                // items amount
                TextMeshProUGUI text = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                text.text = item.Amount.ToString();
                amountsText.Add(text);
            }
        }

        public void Craft()
        {
            craftSystem.Craft(craft);
        }

        public void UpdateUI()
        {
            for (int i = 0; i < craft.ObjectCraft.Count; i++)
            {
                ItemData curCraft = craft.ObjectCraft[i];
                ItemData inventoryItem = PlayerInventory.GetItem(curCraft.Item);
                if(inventoryItem != null)
                {
                    amountsText[i].color = inventoryItem.Amount < curCraft.Amount ? Color.red : Color.white;
                }
                else
                {
                    amountsText[i].color = Color.red;
                }
            }
        }

        #region UI Actions
        public void OnEnterPointer()
        {
            UpdateUI();

            anim.SetBool("enabled", true);
        }
        public void OnExitPointer()
        {
            anim.SetBool("enabled", false);
        }
        #endregion
    }
}
