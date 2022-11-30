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

        [Header("Node Variable")]
        [ReadOnly, Expandable] public CSCraftSO node;

        [Header("Variables")]
        public bool isUnlocked = false;


        [Header("UI")]
        [SerializeField] private TextMeshProUGUI craft_name;
        [SerializeField] private Image craft_icon;
        [Space]
        [SerializeField] private TextMeshProUGUI craft_cost;
        [Space]
        [SerializeField] private CanvasGroup canvasGroup;
        [Space]
        [SerializeField] private GameObject lockSprite;

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
            SetLock(isUnlocked);
        }

        public void SetLock(bool value)
        {
            isUnlocked = value;

            if (isUnlocked)
            {
                lockSprite.SetActive(false);
                canvasGroup.alpha = 1f;
            }
            else
            {
                lockSprite.SetActive(true);
                canvasGroup.alpha = 0.6f;
            }

            foreach (CSCraftChoiceData item in node.Choices)
            {
                CSCraftUIObject obj = craftSystem.GetCraftObj(item.NextDialogue);
                if (obj != null)
                {
                    obj.SetLock(true);
                }
            }
        }
    }
}
