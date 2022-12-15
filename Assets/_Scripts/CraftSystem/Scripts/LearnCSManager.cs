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
        public Transform techTreeRenderTransform;
        public Transform techTreeArrowRenderTransform;
    }

    public class LearnCSManager : MonoBehaviour
    {
        [Header("Tech Tree")]
        [SerializeField] private List<TechTree> techTrees = new List<TechTree>();
        private List<CSCraftSO> unlockedCrafts;

        [Header("Scale")]
        [SerializeField] private float currentScale = 1;
        [Space]
        [SerializeField] private float minTreeScale = 0.5f;
        [SerializeField] private float maxTreeScale = 1;
        [Space]
        [SerializeField] private float sensitivity = 0.2f;

        [Header("UI/Panels")]
        [SerializeField] private GameObject techTreePanel;

        [Header("UI/Prefabs")]
        [SerializeField] private CSCraftUILearn learnCraftPrefab;
        [SerializeField] private CSCraftUIArrow arrowPrefab;

        [Header("Other")]
        [SerializeField] private Canvas canvas;
        public bool isOpened = true;
        public TechTree openedTechTree;

        CSManager craftManager;
        PlayerController player;

        private void Start()
        {
            foreach (var item in LoadCraftUtility.loadCraftUtility.allUnlockedCrafts)
            {
                unlockedCrafts.Add(item);
            }

            InitializeCraftSystem();

            craftManager = FindObjectOfType<CSManager>();
            player = FindObjectOfType<PlayerController>();
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
            craftManager.LearnItem(craft, GetTechTreeByNode(craft).techTree);

            LoadCraftUtility.loadCraftUtility.AddUnlockedCraft(craft);
            unlockedCrafts.Add(craft);
        }

        #region UI Actions
        public void OpenMenu()
        {
            techTreePanel.SetActive(true);
            isOpened = true;
        }
        public void CloseMenu()
        {
            techTreePanel.SetActive(false);
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

                SpawnNodes(tree);
                SpawnConnections(tree);
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

            foreach (CSCraftSO craftData in tree.techTree.UngroupedDialogues)
            {
                CSCraftUILearn obj = Instantiate(learnCraftPrefab.gameObject, tree.techTreeRenderTransform).GetComponent<CSCraftUILearn>();
                obj.Initialize(craftData, new Vector2(craftData.Position.x, -craftData.Position.y));
                tree.loadedLearnCraftPrefabs.Add(obj);
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
    }
}
