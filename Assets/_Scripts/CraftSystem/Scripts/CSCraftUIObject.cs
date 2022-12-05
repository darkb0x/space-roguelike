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

    public class CSCraftUIObject : MonoBehaviour
    {
        [HideInInspector] public RectTransform rectTransform;
        CSManager craftSystem;
        private float currentProgress = 0f;
        private bool isMouseEnterToCraftButton = false;

        [Header("Node Variable")]
        [ReadOnly, Expandable] public CSCraftSO craft;

        [Header("Variables")]
        public bool isUnlocked = false;
        public bool isPursached = false;
        [Space]
        [SerializeField] private Animator anim;
        [Space]
        [SerializeField] private float maxProgress = 3f;

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
        [SerializeField] private Image craftButtonImage;
        [SerializeField] private Button craftButton;
        [Space]
        [SerializeField] private Transform itemListTransform;
        [SerializeField] private GameObject itemListComponent;

        public void Initialize(CSCraftSO data, Vector2 position)
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

            craftButtonImage.fillAmount = currentProgress / maxProgress;
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
                List<CSCraftUIObject> lastCrafts = craftSystem.GetCraftObjInChoices(craft);
                if (lastCrafts != null)
                {
                    foreach (CSCraftUIObject item in lastCrafts)
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
        }

        private void Update()
        {
            if(isMouseEnterToCraftButton)
            {
                if(Input.GetMouseButton(0))
                {
                    Debug.Log(currentProgress);
                    currentProgress += Time.deltaTime;
                    craftButtonImage.fillAmount = currentProgress / maxProgress;

                    if (currentProgress >= maxProgress)
                    {
                        craftSystem.Craft(craft);
                        currentProgress = 0;
                    }
                }
                else
                {
                    currentProgress = Mathf.Lerp(currentProgress, 0, 0.15f);
                    craftButtonImage.fillAmount = currentProgress / maxProgress;
                }
            }
            else
            {
                currentProgress = Mathf.Lerp(currentProgress, 0, 0.15f);
                craftButtonImage.fillAmount = currentProgress / maxProgress;
            }
        }

        private void fullUnlock()
        {
            lockSprite.SetActive(false);
            priceGameObject.SetActive(false);

            craftButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);

            canvasGroup.alpha = 1f;
        }
        private void fullLock()
        {
            lockSprite.SetActive(true);
            priceGameObject.SetActive(true);

            craftButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(true);

            canvasGroup.alpha = 0.5f;
        }
        private void unlockForBuy()
        {
            lockSprite.SetActive(false);
            isUnlocked = true;
        }

        public void Buy()
        {
            if (PlayerInventory.playerInventory.money < craft.CraftCost)
            {
                return;
            }
            if(!isUnlocked)
            {
                return;
            }
            List<CSCraftUIObject> lastCrafts = craftSystem.GetCraftObjInChoices(craft);
            bool canBuy = false;
            if (lastCrafts != null)
            {
                foreach (CSCraftUIObject item in lastCrafts)
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

                    CSCraftUIObject obj = craftSystem.GetCraftObj(item.NextCraft);
                    if (obj != null)
                    {
                        obj.unlockForBuy();
                    }
                }

                PlayerInventory.playerInventory.money -= craft.CraftCost;
                fullUnlock();
            }
        }

        #region UI Actions
        public void OnEnterPointer()
        {
            if (isPursached)
            {
                anim.SetBool("enabled", true);
            }
        }
        public void OnExitPointer()
        {
            if (isPursached)
            {
                anim.SetBool("enabled", false);
            }
        }


        public void OnEnterPointer_Craft()
        {
            isMouseEnterToCraftButton = true;
        }
        public void OnExitPointer_Craft()
        {
            isMouseEnterToCraftButton = false;
        }
        #endregion
    }
}
