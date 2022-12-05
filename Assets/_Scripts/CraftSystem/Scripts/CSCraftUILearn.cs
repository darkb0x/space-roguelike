using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

namespace Game.CraftSystem
{
    using Player.Inventory;
    using CraftSystem.Editor.Data;
    using CraftSystem.Editor.ScriptableObjects;

    public class CSCraftUILearn : MonoBehaviour
    {
        [HideInInspector] public RectTransform rectTransform;
        private CSManager craftSystem;

        [Header("Node Variable")]
        [ReadOnly, Expandable] public CSCraftSO craft;

        [Header("Variables")]
        public bool isUnlocked = false;
        public bool isPursached = false;
        [Space]
        [SerializeField] private Animator anim;
        [SerializeField, AnimatorParam("anim")] private string animValue_enabled;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI craft_name;
        [SerializeField] private Image craft_icon;
        [Space]
        [SerializeField] private TextMeshProUGUI craft_cost;
        [SerializeField] private Button buyButton;
        [SerializeField] private GameObject lockSprite;
        [SerializeField] private GameObject priceGameObject;
        [Space]
        [SerializeField] private CanvasGroup canvasGroup;
        [Space]
        [SerializeField] private Transform itemListTransform;
        [SerializeField] private GameObject itemListComponent;

        public virtual void Initialize(CSCraftSO data, Vector2 position)
        {
            rectTransform = GetComponent<RectTransform>();

            craft = data;
            rectTransform.localPosition = position;

            //UI
            craft_name.text = craft.CraftName;
            craft_icon.sprite = craft.IconSprite;
            craft_cost.text = craft.CraftCost+"$";

            foreach (ItemCraft item in craft.ObjectCraft)
            {
                GameObject obj = Instantiate(itemListComponent, itemListTransform);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = item.item._icon; // item icon
                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.amount.ToString(); // items amount
            }

            //Variables
            isUnlocked = craft.IsStartingNode;
            isPursached = craft.IsStartingNode;

        }

        private void Start()
        {
            craftSystem = FindObjectOfType<CSManager>();

            if (craft.IsStartingNode)
                fullUnlock();
            else
                fullLock();

            if(!craft.IsStartingNode)
            {
                List<CSCraftUILearn> lastCrafts = craftSystem.GetCraftObjInChoices(craft);
                if (lastCrafts != null)
                {
                    foreach (CSCraftUILearn item in lastCrafts)
                    {
                        if (item == null)
                            continue;

                        if (item.isPursached && item.isUnlocked)
                        {
                            unlockForBuy();
                        }
                    }
                }
            }


            if (isPursached)
            {
                craftSystem.AddCraft(craft, craftSystem.GetTechTreeByNode(craft));
            }
        }

        public virtual void Buy()
        {
            if (PlayerInventory.playerInventory.money < craft.CraftCost)
            {
                return;
            }
            if(!isUnlocked)
            {
                return;
            }
            List<CSCraftUILearn> lastCrafts = craftSystem.GetCraftObjInChoices(craft);
            bool canBuy = false;
            if (lastCrafts != null)
            {
                foreach (CSCraftUILearn item in lastCrafts)
                {
                    if (item == null)
                        continue;

                    if (item.isPursached && item.isUnlocked)
                    {
                        canBuy = true;
                    }
                }
            }
            
            if(canBuy)
            {
                isPursached = true;
                foreach (CSCraftChoiceData item in craft.Choices)
                {
                    if (item.NextCraft == null)
                        continue;

                    CSCraftUILearn obj = craftSystem.GetCraftObj(item.NextCraft);
                    if (obj != null)
                    {
                        obj.unlockForBuy();
                    }
                }

                PlayerInventory.playerInventory.money -= craft.CraftCost;
                fullUnlock();
                OnEnterPointer();
                craftSystem.LearnCraft(craft);
            }
        }

        #region Lock Systems
        private void fullUnlock()
        {
            lockSprite.SetActive(false);
            priceGameObject.SetActive(false);

            buyButton.gameObject.SetActive(false);

            canvasGroup.alpha = 1f;
        }
        private void fullLock()
        {
            lockSprite.SetActive(true);
            priceGameObject.SetActive(true);

            buyButton.gameObject.SetActive(true);

            canvasGroup.alpha = 0.5f;
        }
        private void unlockForBuy()
        {
            lockSprite.SetActive(false);
            isUnlocked = true;
        }
        #endregion

        #region UI Actions
        public virtual void OnEnterPointer()
        {
            if (isPursached)
            {
                anim.SetBool("enabled", true);
            }
        }
        public virtual void OnExitPointer()
        {
            if (isPursached)
            {
                anim.SetBool("enabled", false);
            }
        }
        #endregion
    }
}
