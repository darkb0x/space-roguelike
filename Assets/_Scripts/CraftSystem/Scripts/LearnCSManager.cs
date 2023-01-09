using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

namespace Game.CraftSystem
{
    using CraftSystem.Editor.ScriptableObjects;
    using Player.Inventory;
    using Player;

    [System.Serializable]
    public class ItemCraft
    {
        public InventoryItem item;
        public int amount;
    }

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
        [Header("Tech Tree")]
        [SerializeField] private List<TechTree> techTrees = new List<TechTree>();
        private List<CSCraftSO> unlockedCrafts = new List<CSCraftSO>();

        [Header("Scale")]
        [SerializeField] private float currentScale = 1;
        [Space]
        [SerializeField] private float minTreeScale = 0.5f;
        [SerializeField] private float maxTreeScale = 1;
        [Space]
        [SerializeField] private float sensitivity = 0.2f;

        [Header("UI/Panels")]
        [SerializeField] private GameObject techTreePanel;
        [SerializeField] private Transform content;
        [SerializeField] private CSCategoryButtons categoryButtons;

        [Header("UI/Prefabs")]
        [SerializeField] private CSCraftUILearn learnCraftPrefab;
        [SerializeField] private CSCraftUIArrow arrowPrefab;
        [SerializeField] private RectTransform nullObjPrefab;

        [Header("Other")]
        [SerializeField] private Canvas canvas;
        public bool isOpened = false;
        public TechTree openedTechTree;

        PlayerController player;

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();

            foreach (var item in LoadCraftUtility.loadCraftUtility.allUnlockedCrafts)
            {
                unlockedCrafts.Add(item);
            }

            InitializeCraftSystem();

            categoryButtons.Initialize(techTrees, this);

            UIPanelManager.manager.Attach(this);
        }
        private void Update()
        {
            if (isOpened)
            {
                currentScale = Mathf.Clamp(currentScale + Input.mouseScrollDelta.y * sensitivity, minTreeScale, maxTreeScale);
                float scale = Mathf.Lerp(openedTechTree.techTreeRenderTransform.localScale.x, currentScale, sensitivity);
                openedTechTree.techTreeRenderTransform.localScale = new Vector3(scale, scale, 1);

                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseMenu();
                }
            }
        }

        public void LearnCraft(CSCraftSO craft)
        {
            LoadCraftUtility.loadCraftUtility.AddUnlockedCraft(craft);

            if(!unlockedCrafts.Contains(craft))
                unlockedCrafts.Add(craft);
        }

        #region UI Actions
        public void OpenMenu()
        {
            UIPanelManager.manager.OpenPanel(techTreePanel);
            isOpened = true;

            player.canMove = false;
        }
        public void CloseMenu()
        {
            UIPanelManager.manager.ClosePanel(techTreePanel);
            isOpened = false;

            player.canMove = true;
        }
        #endregion

        #region Utilities
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

            foreach (CSCraftSO craftData in tree.techTree.Nodes)
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

        public CSCraftUILearn GetCraftObj(CSCraftSO so)
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
        public List<CSCraftUILearn> GetCraftObjInChoices(CSCraftSO so)
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
        public TechTree GetTechTreeByNode(CSCraftSO nodeSO)
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
    }
}
