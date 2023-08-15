using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;  
using UnityEngine.UI;

namespace Game.CraftSystem.Research.Visual
{
    using CraftSystem.Visual.Category;
    using global::CraftSystem.ScriptableObjects;
    using Node;
    using Input;

    public class ResearchVisual : MonoBehaviour, IUIPanelManagerObserver
    {
        [SerializeField] private Canvas MainCanvas;
        [Space]
        [SerializeField] private GameObject MainPanel;
        [SerializeField] private Transform Content;
        [SerializeField] private ScrollRect MainScrollRect;
        [SerializeField] private CraftCategoryController CategoryController;
        [Space]
        [SerializeField] private Transform TreeVisualsParent;
        [SerializeField] private CraftTreeNodeVisual NodeVisualPrefab;
        [SerializeField] private CraftTreeConnectionVisual ConnectionVisualPrefab;

        private UIPanelManager UIPanelManager;
        private UIInputHandler _input => InputManager.UIInputHandler;
        private ResearchManager _manager;

        private Dictionary<ResearchTree, Dictionary<ResearchTreeCraft, CraftTreeNodeVisual>> _nodes;
        private List<ResearchTree> _trees;

        private ResearchTree _currentResearchTree;
        private float _currentScale;
        private bool _isOpened;

        public void Initalize(List<ResearchTree> trees, ResearchManager manager)
        {
            // Initialize variables
            _trees = trees;
            _manager = manager;
            _nodes = new Dictionary<ResearchTree, Dictionary<ResearchTreeCraft, CraftTreeNodeVisual>>();
            UIPanelManager = Singleton.Get<UIPanelManager>();

            // Create visual
            CreateNodes();
            CreateConnections();

            // Create research tree categories
            Dictionary<ResearchTree, Action> categories = new Dictionary<ResearchTree, Action>();
            foreach (var tree in trees)
            {
                if(tree.Enabled)
                    categories.Add(tree, () => SelectTree(tree));
            }
            CategoryController.Initialize(categories);

            // Choosing research tree
            SelectTree(trees.First(result => result.Enabled));
            Close();

            // Subscribing to events
            _input.CloseEvent += Close;
        }
        private void OnDisable()
        {
            _input.CloseEvent -= Close;
        }

        private void Update()
        {
            if (!_isOpened)
                return;

            // Handlers
            HandleContentPosition();
        }

        #region Public Methods
        public void LoadSaveData(List<CSTreeCraftSO> saveData)
        {
            foreach (var researchTree in _nodes.Keys)
            {
                if (!researchTree.Enabled)
                    continue;

                foreach (var researchCraftTree in _nodes[researchTree].Keys)
                {
                    if (researchCraftTree.LoadData(saveData))
                    {
                        var node = _nodes[researchTree][researchCraftTree];

                        if (researchCraftTree.Upgradable())
                            node.SetState(VisualNodeState.Purchased);
                        else
                            node.SetState(VisualNodeState.FullyUpgraded);
                    }
                }
            }
        }

        public void SelectTree(ResearchTree target)
        {
            _currentResearchTree = target;

            foreach (var tree in _trees)
            {
                if (!tree.Enabled)
                    continue;

                tree.MainVisualParent.gameObject.SetActive(false);
            }

            MainScrollRect.StopMovement();
            Content.localPosition = Vector3.zero;
            SetContentScale(1f);
            _currentResearchTree.MainVisualParent.gameObject.SetActive(true);
        }
        #endregion

        #region Private Methods
        private void SetContentScale(float scale)
        {
            _currentScale = scale;

            Content.localScale = new Vector3(scale, scale, 1);
        }
        #endregion

        #region Handlers
        private void HandleContentPosition()
        {
            Content.localPosition = ClampedPosition();

            Vector2 ClampedPosition()
            {
                var min = _currentResearchTree.ClampedVisualPosition.Min / _currentScale;
                var max = _currentResearchTree.ClampedVisualPosition.Max / _currentScale;
                var pos = Content.localPosition;

                float x = pos.x;
                float y = pos.y;

                // Min
                if (x > Mathf.Abs(min.x))
                {
                    x = Mathf.Abs(min.x);
                }
                if(y < -max.y)
                {
                    y = -max.y;
                }

                // Max
                if(x < -max.x)
                {
                    x = -max.x;
                }
                if(y > Mathf.Abs(min.y))
                {
                    y = Mathf.Abs(min.y);
                }

                return new Vector2(x, y);
            }
        }
        #endregion

        #region Visual Creating
        private void CreateNodes()
        {
            foreach (var researchTree in _trees)
            {
                if (!researchTree.Enabled)
                    continue;

                GameObject treeVisualParent = CreateEmptyGO(researchTree.Title, TreeVisualsParent);
                GameObject connectionsVisualParent = CreateEmptyGO("Connections", treeVisualParent.transform);
                var nodes = InstantiateNodes(researchTree, treeVisualParent.transform);

                researchTree.InjectVisual(nodes.Values.ToList(), treeVisualParent.transform, connectionsVisualParent.transform);

                _nodes.Add(researchTree, nodes);
            }

            GameObject CreateEmptyGO(string name, Transform parent)
            {
                GameObject go = new GameObject(name, typeof(RectTransform));

                go.transform.SetParent(parent);
                go.transform.localPosition = Vector2.zero;
                go.transform.localScale = Vector3.one;

                RectTransform rect = go.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(100, 100); // width and height

                return go;
            }
        }
        private void CreateConnections()
        {
            foreach (var researchTree in _nodes.Keys)
            {
                if (!researchTree.Enabled)
                    continue;

                List<ResearchTreeCraft> currentCraftList = new List<ResearchTreeCraft>(
                    _nodes[researchTree].Keys.ToArray().Where(result => result.IsStartCraft())
                    );
                List<ResearchTreeCraft> nextCraftList = new List<ResearchTreeCraft>();

                while (currentCraftList.Count > 0)
                {
                    foreach (var researchCraftTree in currentCraftList)
                    {
                        List<ResearchTreeCraft> connectedCraftTrees = new List<ResearchTreeCraft>();

                        foreach (var choice in researchCraftTree.crafts.Last().Choices)
                        {
                            ResearchTreeCraft treeCraft = GetTreeCraftByCraft(choice.Next, researchTree);

                            if (treeCraft != null)
                                connectedCraftTrees.Add(treeCraft);
                        }

                        var currentNode = _nodes[researchTree][researchCraftTree];
                        var subsequents = connectedCraftTrees.Select(tree => _nodes[researchTree][tree]).ToList();
                        currentNode.InjectSubsequentNodes(subsequents);
                        foreach (var subsequentNode in subsequents)
                        {
                            var connectionVisual = Instantiate(ConnectionVisualPrefab, researchTree.ConnectionsVisualParent);
                            connectionVisual.SetPosition(currentNode.transform.position, subsequentNode.transform.position, MainCanvas.scaleFactor);
                        }

                        nextCraftList.AddRange(connectedCraftTrees);
                    }

                    currentCraftList = new List<ResearchTreeCraft>(nextCraftList);
                    nextCraftList.Clear();
                }
            }

            ResearchTreeCraft GetTreeCraftByCraft(CSTreeCraftSO craft, ResearchTree researchTree)
            {
                return _nodes[researchTree].Keys.FirstOrDefault(researchTreeCraft => researchTreeCraft.crafts.Contains(craft));
            }
        }

        private Dictionary<ResearchTreeCraft, CraftTreeNodeVisual> InstantiateNodes(ResearchTree researchTree, Transform visualParent)
        {
            Dictionary<ResearchTreeCraft, CraftTreeNodeVisual> result = new Dictionary<ResearchTreeCraft, CraftTreeNodeVisual>();
            CSCraftContainerSO craftTree = researchTree.CraftTree;

            foreach (var craftGroup in craftTree.CraftGroups.Keys)
            {
                ResearchTreeCraft researchTreeCraft = new ResearchTreeCraft(craftTree, craftGroup, craftTree.CraftGroups[craftGroup]);
                CraftTreeNodeVisual node = SpawnNode(researchTreeCraft);

                result.Add(researchTreeCraft, node);
            }

            return result;

            CraftTreeNodeVisual SpawnNode(ResearchTreeCraft researchTreeCraft)
            {
                CraftTreeNodeVisual node = Instantiate(NodeVisualPrefab, visualParent);
                node.transform.localPosition = researchTreeCraft.crafts[0].Position / 2f;
                node.Initalize(researchTreeCraft.group.GroupName, researchTreeCraft, _manager);
                return node;
            }
        }
        #endregion

        #region Window
        public void Open()
        {
            UIPanelManager.OpenPanel(MainPanel);

            _isOpened = true;
        }
        public void Close()
        {
            if (!_isOpened)
                return;

            UIPanelManager.ClosePanel(MainPanel);

            _isOpened = false;
        }

        public void PanelStateIsChanged(GameObject panel)
        {
            if(panel != MainPanel)
            {
                _isOpened = false;
            }
        }
        #endregion
    }
}