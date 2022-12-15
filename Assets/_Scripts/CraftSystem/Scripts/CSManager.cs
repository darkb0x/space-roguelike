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
        [System.Serializable]
        public struct TechTree
        {
            public CSCraftContainerSO techTree;
            [Space]
            public List<CSCraftUICraft> loadedCraftObjects;
            [Space]
            public Transform renderTransform;
        }

        [Header("Crafts")]
        [SerializeField] private List<TechTree> techTrees = new List<TechTree>();

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
            DisSelectCraft();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(isOpened)
                {
                    CloseMenu();
                }
            }
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

            CloseMenu();
        }

        #region UI Interaction
        public void OpenMenu(Workbanch workbanch)
        {
            currentWorkbanch = workbanch;

            craftTreePanel.SetActive(true);
            isOpened = true;
        }
        public void CloseMenu()
        {
            craftTreePanel.SetActive(false);
            isOpened = false;

            player.EndCrafting();
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
            foreach (TechTree tree in techTrees)
            {
                foreach (CSCraftSO item in tree.techTree.UngroupedDialogues)
                {
                    AddItem(item, tree);
                }
            }
        }

        public void LearnItem(CSCraftSO item, CSCraftContainerSO tree)
        {
            TechTree techTree = GetTechTreeByCraftContainer(tree);

            if (techTree.techTree == null)
            {
                Debug.LogError("techTree is null");
            }

            AddItem(item, techTree);
        }

        private void AddItem(CSCraftSO item, TechTree tree)
        {
            CSCraftUICraft obj = Instantiate(craftObjectPrefab.gameObject, tree.renderTransform).GetComponent<CSCraftUICraft>();
            obj.Initialize(item);

            tree.loadedCraftObjects.Add(obj);
        }

        private TechTree GetTechTreeByCraftContainer(CSCraftContainerSO tree)
        {
            foreach (TechTree techTree in techTrees)
            {
                if (techTree.techTree == tree)
                    return techTree;
            }
            return default;
        }
        #endregion
    }
}
