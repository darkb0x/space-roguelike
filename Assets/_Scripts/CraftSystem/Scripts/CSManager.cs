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

    public class CSManager : MonoBehaviour, ICraftListObserver
    {
        [System.Serializable]
        public class CraftTechTree
        {
            public CSCraftContainerSO techTree;
            [Space]
            public List<CSCraftUICraft> loadedCraftObjects;
            public List<CSCraftSO> unlockedCrafts;
            [Space]
            public Sprite categoryIcon;
            [Space]
            [ReadOnly, AllowNesting] public Transform renderTransform;
        }

        [Header("Crafts")]
        [SerializeField] private List<CraftTechTree> techTrees = new List<CraftTechTree>();

        [Header("UI/Panels")]
        [SerializeField] private GameObject craftTreePanel;
        [SerializeField] private Transform craftRenderTransform;
        [Space]
        [SerializeField] private CSCategoryButtons categoryButtons;

        [Header("UI/Prefabs")]
        [SerializeField] private CSCraftUICraft craftObjectPrefab;
        [SerializeField] private GameObject renderTransformPrefab;

        [Header("Other")]
        public bool isOpened = true;

        Workbanch currentWorkbanch;
        PlayerController player;
        CraftTechTree openedTechTree;

        private void Start()
        {
            SpawnObjects();

            player = FindObjectOfType<PlayerController>();
            LoadCraftUtility.loadCraftUtility.AddObserver(this);

            openedTechTree = techTrees[0];

            categoryButtons.Initialize(ConvertListOfTechTree(techTrees));
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
            if (!PlayerInventory.instance.CanTakeItems(craft.ObjectCraft))
                return;

            foreach (var item in craft.ObjectCraft)
            {
                PlayerInventory.instance.TakeItem(item.item, item.amount);
            }

            currentWorkbanch.Craft(craft.GameObjectPrefab);

            CloseMenu();
        }

        #region UI Interaction
        public void OpenMenu(Workbanch workbanch)
        {
            currentWorkbanch = workbanch;

            foreach (var uiElement in openedTechTree.loadedCraftObjects)
            {
                uiElement.UpdateUI();
            }

            UIPanelManager.manager.OpenPanel(craftTreePanel);

            isOpened = true;
        }
        public void CloseMenu()
        {
            UIPanelManager.manager.ClosePanel(craftTreePanel);
            isOpened = false;

            player.ContinuePlayerMove();
        }
        #endregion

        #region Utilities
        private void InitializeCraftSystem()
        {
            foreach (CraftTechTree tree in techTrees)
            {
                foreach (CSCraftSO item in tree.unlockedCrafts)
                {
                    SpawnItem(item, tree);
                }
            }
        }
        private void SpawnObjects()
        {
            foreach (CraftTechTree tree in techTrees)
            {
                if (tree.renderTransform == null)
                {
                    tree.renderTransform = Instantiate(renderTransformPrefab, craftRenderTransform).transform;
                }
            }
        }
        private TechTree ConvertCraftTechTree(CraftTechTree ctt)
        {
            TechTree tt = new TechTree()
            {
                techTree = ctt.techTree,
                techTreeRenderTransform = ctt.renderTransform,
                categoryIcon = ctt.categoryIcon
            };
            return tt;
        }
        private List<TechTree> ConvertListOfTechTree(List<CraftTechTree> lCtt)
        {
            List<TechTree> lTt = new List<TechTree>();
            foreach (var item in lCtt)
            {
                lTt.Add(ConvertCraftTechTree(item));
            }
            return lTt;
        }
        private void SpawnItem(CSCraftSO item, CraftTechTree tree)
        {
            CSCraftUICraft obj = Instantiate(craftObjectPrefab.gameObject, tree.renderTransform).GetComponent<CSCraftUICraft>();
            obj.Initialize(item, this);

            tree.loadedCraftObjects.Add(obj);
        }

        private CraftTechTree GetTechTreeByCraftContainer(CSCraftContainerSO tree)
        {
            foreach (CraftTechTree techTree in techTrees)
            {
                if (techTree.techTree == tree)
                    return techTree;
            }
            return default;
        }
        #endregion

        #region Interface
        public void Initialize(List<CSCraftSO> crafts)
        {
            foreach (CSCraftSO item in crafts)
            {
                if (item == null)
                    continue;

                CraftTechTree currentTechTree = default;
                foreach (var tree in techTrees)
                {
                    if (tree.techTree.Nodes.Contains(item))
                    {
                        currentTechTree = GetTechTreeByCraftContainer(tree.techTree);
                        break;
                    }
                }
                currentTechTree.unlockedCrafts.Add(item);
            }

            InitializeCraftSystem();
        }
        public void GetNewCraft(LoadCraftUtility craftUtility, CSCraftSO newCraft)
        {
            CraftTechTree techTree = default;
            foreach (var tree in techTrees)
            {
                if (tree.techTree.Nodes.Contains(newCraft))
                {
                    techTree = GetTechTreeByCraftContainer(tree.techTree);
                    break;
                }
            }

            SpawnItem(newCraft, techTree);
        }
        #endregion
    }
}
