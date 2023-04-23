using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using TMPro;

namespace Game.CraftSystem
{
    using Player.Inventory;
    using CraftSystem.Editor.Data;
    using CraftSystem.Editor.ScriptableObjects;

    public class CSCraftUILearn : MonoBehaviour
    {
        [Header("Node Variable")]
        [ReadOnly, Expandable] public CSCraftSOTree craft;

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
        [SerializeField] private GameObject lockSprite;
        [SerializeField] private GameObject priceGameObject;
        [Space]
        [SerializeField] private Image learnButtonImage;
        [SerializeField] private GameObject learnButton;
        [SerializeField] private float maxProgress;
        [Space]
        [SerializeField] private CanvasGroup canvasGroup;
        [Space]
        [SerializeField] private Transform itemListTransform;
        [SerializeField] private GameObject itemListComponent;

        [HideInInspector] public RectTransform rectTransform;
        private LearnCSManager learnCraftSystem;
        private float currentProgress = 0f;
        private bool isMouseEnterToCraftButton = false;
        private Color startPriceTextColor;
        private Color noMoneyColor = Color.red;

        private PlayerInventory PlayerInventory;

        public void Initialize(CSCraftSOTree data, Vector2 position, LearnCSManager manager)
        {
            PlayerInventory = Singleton.Get<PlayerInventory>();
            learnCraftSystem = manager;

            rectTransform = GetComponent<RectTransform>();

            craft = data;
            rectTransform.localPosition = position;

            //UI
            startPriceTextColor = craft_cost.color;

            craft_name.text = craft.CraftName;
            craft_icon.sprite = craft.IconSprite;
            craft_cost.text = craft.CraftCost+"$";

            foreach (ItemData item in craft.ObjectCraft)
            {
                if(item == null | item.Item == null)
                {
                    continue;
                }

                GameObject obj = Instantiate(itemListComponent, itemListTransform);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = item.Item.Icon; // item icon
                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.Amount.ToString(); // items amount
            }

            //Variables
            isUnlocked = craft.IsStartingNode;
            isPursached = craft.IsStartingNode;
        }

        public void InitializeOnStart()
        {
            fullLock();

            if (craft.IsStartingNode)
            {
                fullUnlock();
                learnCraftSystem.LearnCraft(craft);
            }
            if (isUnlocked)
            {
                unlockForBuy();
            }
            if(isPursached)
            {
                fullUnlock();
            }
        }

        private void Update()
        {
            if (isMouseEnterToCraftButton)
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    float addedValue = Time.deltaTime;
                    currentProgress += addedValue; 
                    learnButtonImage.fillAmount = currentProgress / maxProgress;

                    if (currentProgress >= maxProgress)
                    {
                        currentProgress = 0;
                        Buy();
                    }
                }
                else
                {
                    currentProgress = Mathf.Lerp(currentProgress, 0, 0.15f);
                    learnButtonImage.fillAmount = currentProgress / maxProgress;
                }
            }
            else
            {
                currentProgress = Mathf.Lerp(currentProgress, 0, 0.15f);
                learnButtonImage.fillAmount = currentProgress / maxProgress;
            }
        }

        public virtual void Buy()
        {
            if (PlayerInventory.money < craft.CraftCost)
            {
                StartCoroutine(NoMoneyAnimation());
                return;
            }
            if(!isUnlocked)
            {
                return;
            }
            List<CSCraftUILearn> lastCrafts = learnCraftSystem.GetCraftObjInChoices(craft);
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
                PlayerInventory.money -= craft.CraftCost;
                fullUnlock();
                OnEnterPointer();
                learnCraftSystem.LearnCraft(craft);
            }
        }

        #region Lock Systems
        public void fullUnlock()
        {
            lockSprite.SetActive(false);
            priceGameObject.SetActive(false);

            canvasGroup.alpha = 1f;

            learnButton.SetActive(false);
            isMouseEnterToCraftButton = false;

            isPursached = true;
            isUnlocked = true;

            foreach (CSCraftChoiceData item in craft.Choices)
            {
                if (item.NextCraft == null)
                    continue;

                CSCraftUILearn obj = learnCraftSystem.GetCraftObj(item.NextCraft);
                if (obj != null)
                {
                    obj.unlockForBuy();
                }
            }
        }
        public void fullLock()
        {
            lockSprite.SetActive(true);
            priceGameObject.SetActive(true);

            canvasGroup.alpha = 0.5f;

            learnButton.SetActive(true);
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

        public void OnEnterPointer_learn()
        {
            if(isUnlocked)
            {
                isMouseEnterToCraftButton = true;
            }
        }
        public void OnExitPointer_learn()
        {
            if (isUnlocked)
            {
                isMouseEnterToCraftButton = false;
            }
        }

        private IEnumerator NoMoneyAnimation()
        {
            float changeSpeed = 3;

            craft_cost.color = noMoneyColor;

            while(craft_cost.color != startPriceTextColor)
            {
                Color current = new Color()
                {
                    r = Mathf.MoveTowards(craft_cost.color.r, startPriceTextColor.r, changeSpeed * Time.deltaTime),
                    g = Mathf.MoveTowards(craft_cost.color.g, startPriceTextColor.g, changeSpeed * Time.deltaTime),
                    b = Mathf.MoveTowards(craft_cost.color.b, startPriceTextColor.b, changeSpeed * Time.deltaTime),
                    a = 1
                };
                craft_cost.color = current;

                yield return null;
            }
        }
        #endregion
    }
}
