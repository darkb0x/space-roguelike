using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace Game.CraftSystem
{
    using CraftSystem.Editor.ScriptableObjects;
    using Player.Inventory;
    using Player;
    using SaveData;
    using Utilities.Notifications;

    public class CSManager : MonoBehaviour, IUIPanelManagerObserver
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
        [System.Serializable]
        public class CraftUnique
        {
            public List<CSCraftUICraft> loadedCraftObjects;
            public List<CSCraftSO> unlockedCrafts;
            [Space]
            public Sprite categoryIcon;
            [Space]
            public Transform renderTransform;
        }

        [Header("Crafts")]
        [SerializeField] private List<CraftTechTree> techTrees = new List<CraftTechTree>();
        [SerializeField] private CraftUnique UniqueCrafts;

        [Header("UI/Panels")]
        [SerializeField, Tooltip("Canvas/Craft list")] private GameObject craftTreePanel;
        [SerializeField, Tooltip("Canvas/Craft list/Scroll View/Viewport/Content")] private Transform craftRenderTransform;
        [Space]
        [SerializeField, Tooltip("Canvas/Craft list/Categories")] private CSCategoryButtons categoryButtons;

        [Header("UI/Prefabs")]
        [SerializeField] private CSCraftUICraft craftObjectPrefab;
        [SerializeField] private GameObject renderTransformPrefab;

        [Header("Other")]
        public bool isOpened = true;

        private Workbanch currentWorkbanch;
        private PlayerController player;
        private CraftTechTree openedTechTree;
        private SessionData currentSessionData => GameData.Instance.CurrentSessionData;
        private UIPanelManager UIPanelManager;
        private PlayerInventory PlayerInventory;

        private void Awake()
        {
            SpawnObjects();
        }

        private void Start()
        {
            UIPanelManager = Singleton.Get<UIPanelManager>();
            PlayerInventory = Singleton.Get<PlayerInventory>();

            GameInput.InputActions.UI.CloseWindow.performed += CloseMenu;
            UIPanelManager.Attach(this);

            player = FindObjectOfType<PlayerController>();

            List<TechTree> trees = ConvertListOfTechTree(techTrees);
            TechTree UniqueCraftsTechTree = new TechTree()
            {
                techTree = null,
                techTreeRenderTransform = UniqueCrafts.renderTransform,
                categoryIcon = UniqueCrafts.categoryIcon
            };
            trees.Add(UniqueCraftsTechTree);
            categoryButtons.Initialize(trees);
            openedTechTree = techTrees[0];

            InitializeCraftSystem();

            LearnCSManager learnCSManager = Singleton.Get<LearnCSManager>();
            if (learnCSManager != null)
            {
                learnCSManager.OnCraftLearned += GotNewCraft;
            }
        }

        public void Craft(CSCraftSO craft)
        {
            if (!PlayerInventory.CanTakeItems(craft.ObjectCraft))
                return;

            PlayerInventory.TakeItem(craft.ObjectCraft);

            currentWorkbanch.Craft(craft.ObjectPrefab);

            NotificationManager.NewNotification(craft.IconSprite, "Crafted!", false);
            LogUtility.WriteLog($"Crafted: {craft.CraftName} (path: {craft.AssetPath})");

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

            UIPanelManager.OpenPanel(craftTreePanel);

            isOpened = true;

            player.StopPlayerMove();
        }
        private void CloseMenu()
        {
            if (!isOpened)
                return;

            UIPanelManager.ClosePanel(craftTreePanel);
            isOpened = false;

            player.ContinuePlayerMove();
        }
        private void CloseMenu(InputAction.CallbackContext context)
        {
            CloseMenu();
        }
        #endregion

        #region Utilities
        private void InitializeCraftSystem()
        {
            List<CSCraftSO> crafts = GetCrafts();
            foreach (CSCraftSO item in crafts)
            {
                if (item == null)
                    continue;

                if(item is CSCraftSOTree itemTree)
                {
                    CraftTechTree currentTechTree = default;
                    foreach (var tree in techTrees)
                    {
                        if (tree.techTree.Nodes.Contains(itemTree))
                        {
                            currentTechTree = GetTechTreeByCraftContainer(tree.techTree);
                            break;
                        }
                    }

                    if (!currentTechTree.unlockedCrafts.Contains(item))
                        currentTechTree.unlockedCrafts.Add(item);
                }
                else
                {
                    if(!UniqueCrafts.unlockedCrafts.Contains(item))
                    {
                        UniqueCrafts.unlockedCrafts.Add(item);
                    }
                }
            }

            foreach (CraftTechTree tree in techTrees)
            {
                foreach (CSCraftSOTree item in tree.unlockedCrafts)
                {
                    SpawnItem(item, tree);
                }
            }
            foreach (CSCraftSO item in UniqueCrafts.unlockedCrafts)
            {
                SpawnItem(item);
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
        private void SpawnItem(CSCraftSO item)
        {
            CSCraftUICraft obj = Instantiate(craftObjectPrefab.gameObject, UniqueCrafts.renderTransform).GetComponent<CSCraftUICraft>();
            obj.Initialize(item, this);
            UniqueCrafts.loadedCraftObjects.Add(obj);
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

        private List<CSCraftSO> GetCrafts()
        {
            List<CSCraftSO> crafts = new List<CSCraftSO>();
            foreach (var craft in currentSessionData.UnlockedCraftPaths)
            {
                crafts.Add(currentSessionData.GetCraft(craft));
            }
            return crafts;
        }
        #endregion

        public void GotNewCraft(CSCraftSO newCraft)
        {
            if(newCraft is CSCraftSOTree craftTree)
            {
                CraftTechTree techTree = default;
                foreach (var tree in techTrees)
                {
                    if (tree.techTree.Nodes.Contains(craftTree))
                    {
                        if (tree.unlockedCrafts.Contains(craftTree))
                            return;

                        techTree = GetTechTreeByCraftContainer(tree.techTree);
                        break;
                    }
                }

                SpawnItem(newCraft, techTree);
            }
            else
            {
                if (UniqueCrafts.unlockedCrafts.Contains(newCraft))
                    return;

                SpawnItem(newCraft);
            }
        }

        private void OnDisable()
        {
            GameInput.InputActions.UI.CloseWindow.performed -= CloseMenu;
        }

        public void PanelStateIsChanged(GameObject panel)
        {
            if(panel != craftTreePanel)
            {
                if(isOpened)
                {
                    isOpened = false;
                }
            }
        }
    }
}
