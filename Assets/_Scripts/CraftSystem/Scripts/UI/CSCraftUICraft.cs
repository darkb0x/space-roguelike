using System.Collections;
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
        private float currentProgress = 0f;

        [Header("Variables")]
        [ReadOnly, Expandable] public CSCraftSO craft;
        [Space]
        [SerializeField] private float maxProgress = 3f;
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

        public void Initialize(CSCraftSO craftSO, CSManager manager)
        {
            // Variables
            craft = craftSO;

            craftButtonImage.fillAmount = currentProgress / maxProgress;

            craftSystem = manager;

            //UI
            craft_name.text = craft.CraftName;
            craft_icon.sprite = craft.IconSprite;

            foreach (ItemCraft item in craft.ObjectCraft)
            {
                GameObject obj = Instantiate(itemListComponent, itemListTransform);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = item.item._icon; // item icon
                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.amount.ToString(); // items amount
            }
        }

        public void Craft()
        {
            craftSystem.Craft(craft);
        }

        public void UpdateUI()
        {
            if (itemListTransform.childCount > 0)
            {
                if (itemListTransform.childCount >= 0)
                {
                    int childCount = itemListTransform.childCount;
                    for (int i = childCount - 1; i > 0; i--)
                    {
                        Destroy(itemListTransform.gameObject);
                    }
                }
            }

            foreach (ItemCraft item in craft.ObjectCraft)
            {
                GameObject obj = Instantiate(itemListComponent, itemListTransform);

                // item icon
                Image icon = obj.transform.GetChild(0).GetComponent<Image>();
                icon.sprite = item.item._icon;

                // items amount
                TextMeshProUGUI amount = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                amount.text = item.amount.ToString();

                if (PlayerInventory.playerInventory.GetItem(item.item).amount < item.amount)
                {
                    amount.color = Color.red;
                }
                else
                {
                    amount.color = Color.white;
                }
            }
        }

        #region UI Actions
        public void OnEnterPointer()
        {
            anim.SetBool("enabled", true);

            craftSystem.SelectACraft(craft);
        }
        public void OnExitPointer()
        {
            anim.SetBool("enabled", false);

            craftSystem.DisSelectCraft();
        }
        #endregion
    }
}
