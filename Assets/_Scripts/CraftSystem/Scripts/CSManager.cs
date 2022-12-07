using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace Game.CraftSystem
{
    using CraftSystem.Editor.ScriptableObjects;
    using Player.Inventory;

    public class CSManager : MonoBehaviour
    {
        [Header("Crafts")]
        [SerializeField] private CSCraftSO[] craftList;
        [SerializeField] private List<CSCraftUICraft> loadedCraftObjects;

        [Header("UI/Panels")]
        [SerializeField] private GameObject craftTreePanel;
        [SerializeField] private Transform craftRenderTransform;

        [Header("UI/Craft")]
        [SerializeField] private Image currentWorkbanchImage;
        [SerializeField] private Image currentItemImage;

        [Header("UI/Prefabs")]
        [SerializeField] private CSCraftUICraft craftObjectPrefab;

        [Header("Other")]
        public bool isOpened = true;

        Workbanch currentWorkbanch;

        private void Start()
        {
            InitializeCraftSystem();
        }

        public void OpenCraftMenu(Workbanch workbanch)
        {
            currentWorkbanch = workbanch;

            craftTreePanel.SetActive(true);
            isOpened = true;
        }
        public void CloseCraftMenu()
        {
            craftTreePanel.SetActive(false);
            isOpened = false;
        }

        public void Craft(CSCraftSO craft)
        {
            if (!PlayerInventory.playerInventory.CanTakeItems(craft.ObjectCraft))
                return;

            // Craft on workbanch
        }

        public void SelectACraft(CSCraftSO craft)
        {
            currentItemImage.color = new Color(1, 1, 1, 1);
            currentItemImage.sprite = craft.IconSprite;
        }
        public void DisSelectCraft()
        {
            currentItemImage.color = new Color(1, 1, 1, 0);
        }

        #region Utilities
        private void InitializeCraftSystem()
        {
            foreach (CSCraftSO item in craftList)
            {
                CSCraftUICraft obj = Instantiate(craftObjectPrefab.gameObject, craftRenderTransform).GetComponent<CSCraftUICraft>();
                obj.Initialize(item);

                loadedCraftObjects.Add(obj);
            }
        }
        #endregion
    }
}
