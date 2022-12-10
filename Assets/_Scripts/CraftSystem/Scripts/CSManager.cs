using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace Game.CraftSystem
{
    using CraftSystem.Editor.ScriptableObjects;
    using Player.Inventory;
    using Player;

    public class CSManager : MonoBehaviour
    {
        [Header("Crafts")]
        [SerializeField] private CSCraftContainerSO techTree;
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
        PlayerController player;

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();

            InitializeCraftSystem();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(isOpened)
                {
                    CloseCraftMenu();
                }
            }
        }

        #region UI Interaction
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

            player.EndCrafting();
        }

        public void Craft(CSCraftSO craft)
        {
            if (!PlayerInventory.playerInventory.CanTakeItems(craft.ObjectCraft))
                return;

            foreach (var item in craft.ObjectCraft)
            {
                PlayerInventory.playerInventory.TakeItem(item.item, item.amount);
            }

            currentWorkbanch.Craft(craft.GameObjectPrefab);

            CloseCraftMenu();
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
        #endregion

        #region Utilities
        private void InitializeCraftSystem()
        {
            foreach (CSCraftSO item in techTree.UngroupedDialogues)
            {
                CSCraftUICraft obj = Instantiate(craftObjectPrefab.gameObject, craftRenderTransform).GetComponent<CSCraftUICraft>();
                obj.Initialize(item);

                loadedCraftObjects.Add(obj);
            }
        }
        #endregion
    }
}
