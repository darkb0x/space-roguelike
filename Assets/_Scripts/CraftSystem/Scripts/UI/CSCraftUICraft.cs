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
        List<TextMeshProUGUI> amountsText = new List<TextMeshProUGUI>();

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
                if (item == null | item.item == null)
                {
                    Debug.Log(craft.AssetPath + " Object craft have null item, please fix it!");
                    continue;
                }

                GameObject obj = Instantiate(itemListComponent, itemListTransform);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = item.item._icon; // item icon

                // items amount
                TextMeshProUGUI text = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                text.text = item.amount.ToString();
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
                ItemCraft curCraft = craft.ObjectCraft[i];
                amountsText[i].color = PlayerInventory.playerInventory.GetItem(curCraft.item).amount < curCraft.amount ? Color.red : Color.white;
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
