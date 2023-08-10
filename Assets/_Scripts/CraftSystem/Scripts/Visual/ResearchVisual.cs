using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Visual
{
    using global::CraftSystem.ScriptableObjects;
    using Node;

    public class ResearchVisual : MonoBehaviour
    {
        [SerializeField] private Canvas MainCanvas;
        [Space]
        [SerializeField] private Transform TreeVisualsParent;
        [SerializeField] private CraftTreeNodeVisual NodeVisualPrefab;
        [SerializeField] private CraftTreeConnectionVisual ConnectionVisualPrefab;

        private ResearchManager _manager;

        private Dictionary<ResearchTree, Dictionary<ResearchTreeCraft, CraftTreeNodeVisual>> _nodes;
        private List<ResearchTree> _trees;

        public void Initalize(List<ResearchTree> trees, List<CSTreeCraftSO> saveData, ResearchManager manager)
        {
            _trees = trees;
            _manager = manager;
            _nodes = new Dictionary<ResearchTree, Dictionary<ResearchTreeCraft, CraftTreeNodeVisual>>();
 
            // Nodes
            foreach (var researchTree in _trees)
            { 
                GameObject treeVisualParent = CreateEmptyGO(researchTree.Title, TreeVisualsParent);
                GameObject connectionsVisualParent = CreateEmptyGO("Connections", treeVisualParent.transform);

                researchTree.InjectVisual(treeVisualParent.transform, connectionsVisualParent.transform);

                _nodes.Add(researchTree, InstantiateNodes(researchTree, treeVisualParent.transform));
            }
            foreach (var researchTree in _nodes.Keys)
            {
                // Create connections
                List<ResearchTreeCraft> currentCraftList = new List<ResearchTreeCraft>() 
                    { Array.Find(_nodes[researchTree].Keys.ToArray(), result => result.IsStartCraft()) };
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

                // Load data
                foreach (var researchCraftTree in _nodes[researchTree].Keys)
                {
                    if(researchCraftTree.LoadData(saveData))
                    {
                        var node = _nodes[researchTree][researchCraftTree];

                        if (researchCraftTree.Upgradable())
                            node.SetState(VisualNodeState.Purchased);
                        else
                            node.SetState(VisualNodeState.FullyUpgraded);
                    }
                }
            }

            // Functions
            ResearchTreeCraft GetTreeCraftByCraft(CSTreeCraftSO craft, ResearchTree researchTree)
            {
                return _nodes[researchTree].Keys.FirstOrDefault(researchTreeCraft => researchTreeCraft.crafts.Contains(craft));
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
    }
}