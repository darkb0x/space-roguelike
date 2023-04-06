using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Game.CraftSystem
{
    using CraftSystem.Editor.ScriptableObjects;
    using Utilities.Notifications;
    using SaveData;

    public delegate void OnNewCraftLearned(CSCraftSO craft);

    [System.Serializable]
    public class TechTree
    {
        public CSCraftContainerSO techTree;

        [Space]

        //Tech Tree components
        public List<CSCraftUILearn> loadedLearnCraftPrefabs;
        [HideInInspector] public Transform techTreeRenderTransform;
        [HideInInspector] public Transform techTreeArrowRenderTransform;

        [Space]

        public Sprite categoryIcon;
    }

    public class LearnCSManager : MonoBehaviour, ICategoryButtonsChecker, IUIPanelManagerObserver
    {
        public static LearnCSManager Instance;

        [Header("Tech Tree")]
        [SerializeField] private List<TechTree> techTrees = new List<TechTree>();
        private List<CSCraftSOTree> unlockedCrafts = new List<CSCraftSOTree>();

        [Header("Scale")]
        [SerializeField] private float currentScale = 1;
        [SerializeField] private float scaleSpeed = 3f;
        [Space]
        [SerializeField] private float minTreeScale = 0.5f;
        [SerializeField] private float maxTreeScale = 1;

        [Header("UI/Panels")]
        [SerializeField, Tooltip("Canvas/Craft tree")] private GameObject techTreePanel;
        [SerializeField, Tooltip("Canvas/Craft tree/Scroll View/Content")] private Transform content;
        [SerializeField, Tooltip("Canvas/Craft tree/Categories")] private CSCategoryButtons categoryButtons;

        [Header("UI/Prefabs")]
        [SerializeField] private CSCraftUILearn learnCraftPrefab;
        [SerializeField] private CSCraftUIArrow arrowPrefab;
        [SerializeField] private RectTransform nullObjPrefab;

        [Header("Other")]
        [SerializeField, Tooltip("Default canvas")] private Canvas canvas;
        public bool isOpened = false;
        public TechTree openedTechTree;

        public OnNewCraftLearned OnCraftLearned;
        private SessionData currentSessionData => GameData.Instance.CurrentSessionData;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameInput.InputActions.UI.CloseWindow.performed += CloseMenu;

            AddStartingCrafts();
            foreach (var craft in currentSessionData.UnlockedCraftPaths)
            {
                CSCraftSO craftSO = currentSessionData.GetCraft(craft);

                if (craftSO is CSCraftSOTree craftTree)
                {
                    if(!unlockedCrafts.Contains(craftTree))
                        unlockedCrafts.Add(craftTree);
                }
            }

            InitializeCraftSystem();

            categoryButtons.Initialize(techTrees, this);

            UIPanelManager.Instance.Attach(this);
        }
        private void Update()
        {
            if (isOpened)
            {
                float sensitivity = 0.2f;

                currentScale = Mathf.Clamp(currentScale + GameInput.Instance.GetMouseScrollDeltaY() * sensitivity, minTreeScale, maxTreeScale);
                float scale = Mathf.Lerp(openedTechTree.techTreeRenderTransform.localScale.x, currentScale, scaleSpeed * Time.deltaTime);
                openedTechTree.techTreeRenderTransform.localScale = new Vector3(scale, scale, 1);
            }
        }

        public void LearnCraft(CSCraftSOTree craft, bool showNotify = true)
        {
            if (!unlockedCrafts.Contains(craft))
            {
                unlockedCrafts.Add(craft);

                OnCraftLearned?.Invoke(craft);

                if(showNotify)
                {
                    NotificationManager.NewNotification(craft.IconSprite, "New craft!", true);
                }
            }
            if (!currentSessionData.UnlockedCraftPaths.Contains(craft.AssetPath))
            {
                currentSessionData.UnlockedCraftPaths.Add(craft.AssetPath);
                currentSessionData.Save();
            }
        }
        public void LearnCraft(CSCraftSO craft, bool showNotify = true)
        {
            if (!currentSessionData.UnlockedCraftPaths.Contains(craft.AssetPath))
            {
                OnCraftLearned?.Invoke(craft);

                currentSessionData.UnlockedCraftPaths.Add(craft.AssetPath);
                currentSessionData.Save();

                if (showNotify)
                {
                    NotificationManager.NewNotification(craft.IconSprite, "New craft!", true);
                }
            }
        }

        #region UI Actions
        public void OpenMenu()
        {
            UIPanelManager.Instance.OpenPanel(techTreePanel);
            isOpened = true;
        }
        public void CloseMenu()
        {
            UIPanelManager.Instance.ClosePanel(techTreePanel);
            isOpened = false;
        }
        public void CloseMenu(InputAction.CallbackContext context)
        {
            CloseMenu();
        }
        #endregion

        #region Utilities
        private void AddStartingCrafts()
        {
            foreach (TechTree tree in techTrees)
            {
                foreach (var node in tree.techTree.Nodes)
                {
                    if(node.IsStartingNode)
                    {
                        LearnCraft(node, false);
                    }
                }
            }
        }

        [Button]
        private void InitializeCraftSystem()
        {
            foreach (TechTree tree in techTrees)
            {
                tree.loadedLearnCraftPrefabs = new List<CSCraftUILearn>();

                SpawnObjects(tree);
                SpawnNodes(tree);
                SpawnConnections(tree);
            }
        }

        private void SpawnObjects(TechTree tree)
        {
            if(tree.techTreeRenderTransform == null)
            {
                GameObject nodeRender = Instantiate(nullObjPrefab.gameObject, content);
                nodeRender.name = tree.techTree.name;
                tree.techTreeRenderTransform = nodeRender.transform;
            }
            if(tree.techTreeArrowRenderTransform == null)
            {
                GameObject arrowRender = Instantiate(nullObjPrefab.gameObject, tree.techTreeRenderTransform);
                arrowRender.name = "Arrows";
                tree.techTreeArrowRenderTransform = arrowRender.transform;
            }
        }
        private void SpawnNodes(TechTree tree)
        {
            if (tree.techTreeRenderTransform.childCount > 0)
            {
                int childCount = tree.techTreeRenderTransform.childCount;
                for (int i = childCount - 1; i > 0; i--)
                {
                    DestroyImmediate(tree.techTreeRenderTransform.GetChild(i).gameObject);
                }
                tree.loadedLearnCraftPrefabs.Clear();
            }

            foreach (CSCraftSOTree craftData in tree.techTree.Nodes)
            {
                CSCraftUILearn obj = Instantiate(learnCraftPrefab.gameObject, tree.techTreeRenderTransform).GetComponent<CSCraftUILearn>();
                obj.Initialize(craftData, new Vector2(craftData.Position.x, -craftData.Position.y), this);
                tree.loadedLearnCraftPrefabs.Add(obj);
            }
            foreach (CSCraftUILearn node in tree.loadedLearnCraftPrefabs)
            {
                node.InitializeOnStart();

                if (unlockedCrafts.Contains(node.craft))
                    node.fullUnlock();
            }
        }
        private void SpawnConnections(TechTree tree)
        {
            if (tree.techTreeArrowRenderTransform.childCount > 0)
            {
                int childCount = tree.techTreeArrowRenderTransform.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(tree.techTreeArrowRenderTransform.GetChild(i).gameObject);
                }
            }

            foreach (CSCraftUILearn craftUIObject in tree.loadedLearnCraftPrefabs)
            {
                foreach (var choice in craftUIObject.craft.Choices)
                {
                    if (choice.NextCraft == null)
                        continue;

                    CSCraftUIArrow objArrow = Instantiate(arrowPrefab.gameObject, tree.techTreeArrowRenderTransform).GetComponent<CSCraftUIArrow>();
                    objArrow.SetPosition(craftUIObject.rectTransform.position, GetCraftObj(choice.NextCraft).rectTransform.position, canvas.scaleFactor);
                }
            }
        }

        public CSCraftUILearn GetCraftObj(CSCraftSOTree so)
        {
            foreach (CSCraftUILearn uiObj in GetTechTreeByNode(so).loadedLearnCraftPrefabs)
            {
                if (uiObj.craft == so)
                {
                    return uiObj;
                }
            }
            return null;
        }
        public List<CSCraftUILearn> GetCraftObjInChoices(CSCraftSOTree so)
        {
            List<CSCraftUILearn> objectList = new List<CSCraftUILearn>();

            foreach (CSCraftUILearn uiObj in GetTechTreeByNode(so).loadedLearnCraftPrefabs)
            {
                if (uiObj.craft.Choices.Count > 0 && uiObj.craft.Choices != null)
                {
                    foreach (var choice in uiObj.craft.Choices)
                    {
                        if (choice.NextCraft == so)
                        {
                            objectList.Add(uiObj);
                        }
                    }
                }
            }

            if (objectList != null && objectList.Count > 0)
                return objectList;
            else
                return null;
        }
        public TechTree GetTechTreeByNode(CSCraftSOTree nodeSO)
        {
            foreach (TechTree tree in techTrees)
            {
                foreach (CSCraftUILearn item in tree.loadedLearnCraftPrefabs)
                {
                    if (item.craft == nodeSO)
                    {
                        return tree;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Interfaces
        public void OpenedPanel(RectTransform panelTransform)
        {
            foreach (var tree in techTrees)
            {
                if(tree.techTreeRenderTransform == panelTransform.transform)
                {
                    openedTechTree = tree;
                }
            }
        }

        public void PanelStateIsChanged(GameObject panel)
        {
            if(panel == techTreePanel)
            {
                if (!techTreePanel.activeSelf)
                    return;

                content.localPosition = Vector3.zero;
            }
        }
        #endregion

        private void OnDisable()
        {
            GameInput.InputActions.UI.CloseWindow.performed -= CloseMenu;
        }
    }
}
