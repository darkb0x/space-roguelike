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
        RectTransform rectTransform;
        CSManager craftSystem;
        float currentProgress = 0f;
        bool isPressed = false;

        [Header("Node Variable")]
        [ReadOnly, Expandable] public CSCraftSO node;

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
        [Space]
        [SerializeField] private CanvasGroup canvasGroup;
        [Space]
        [SerializeField] private GameObject lockSprite;
        [SerializeField] private GameObject priceGameObject;
        [Space]
        [SerializeField] private Image craftButtonImage;

        public void Initialize(CSCraftSO data, Vector2 position, CSManager manager)
        {
            rectTransform = GetComponent<RectTransform>();

            node = data;
            rectTransform.localPosition = position;
            craftSystem = manager;

            //UI
            craft_name.text = node.CraftName;
            craft_icon.sprite = node.IconSprite;
            craft_cost.text = node.CraftCost+"$";

            //Variables
            isUnlocked = node.IsStartingNode;
            isPursached = node.IsStartingNode;

            craftButtonImage.fillAmount = currentProgress / maxProgress;
        }

        private void Start()
        {
            SetLock(isUnlocked);
        }

        private void Update()
        {
            if(isPressed)
            {
                if(Input.GetMouseButton(0))
                {
                    Debug.Log(currentProgress);
                    currentProgress += Time.deltaTime;
                    craftButtonImage.fillAmount = currentProgress / maxProgress;

                    if (currentProgress >= maxProgress)
                    {
                        craftSystem.Craft(node);
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

        public void SetLock(bool value)
        {
            isUnlocked = value;

            if (isUnlocked)
            {
                lockSprite.SetActive(false);
                canvasGroup.alpha = 1f;
                priceGameObject.SetActive(false);
            }
            else
            {
                lockSprite.SetActive(true);
                canvasGroup.alpha = 0.6f;
                priceGameObject.SetActive(true);
            }
        }

        public void Buy()
        {
            foreach (CSCraftChoiceData item in node.Choices)
            {
                if (item.NextCraft == null)
                    continue;

                CSCraftUIObject obj = craftSystem.GetCraftObj(item.NextCraft);
                if (obj != null)
                {
                    obj.isPursached = true;
                    obj.SetLock(true);
                }
            }
        }

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
            isPressed = true;
        }
        public void OnExitPointer_Craft()
        {
            isPressed = false;
        }

    }
}
