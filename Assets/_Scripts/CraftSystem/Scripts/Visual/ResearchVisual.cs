using System.Collections;
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
        [SerializeField] private Transform TreeVisualsParent;
        [SerializeField] private CraftTreeNodeVisual NodeVisualPrefab;

        private ResearchManager _manager;

        private Dictionary<ResearchTree, Dictionary<ResearchTreeCraft, CraftTreeNodeVisual>> _nodes;
        private List<ResearchTree> _trees;

        public void Initalize(List<ResearchTree> trees, ResearchManager manager)
        {
            _trees = trees;
            _manager = manager;
            _nodes = new Dictionary<ResearchTree, Dictionary<ResearchTreeCraft, CraftTreeNodeVisual>>();
 
            // Main nodes
            foreach (var researchTree in _trees)
            { 
                GameObject treeVisualParent = CreateEmptyGO(researchTree.title, TreeVisualsParent);

                _nodes.Add(researchTree, InstantiateNodes(researchTree, treeVisualParent.transform));
            }
            // Connections
            foreach (var researchTree in _nodes.Keys)
            {
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

                        nextCraftList.AddRange(connectedCraftTrees);
                    }

                    currentCraftList = nextCraftList;
                    nextCraftList.Clear();
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

                RectTransform rect = go.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(100, 100); // width and height

                return go;
            }
        }

        private Dictionary<ResearchTreeCraft, CraftTreeNodeVisual> InstantiateNodes(ResearchTree researchTree, Transform visualParent)
        {
            Dictionary<ResearchTreeCraft, CraftTreeNodeVisual> result = new Dictionary<ResearchTreeCraft, CraftTreeNodeVisual>();
            CSCraftContainerSO craftTree = researchTree.craftTree;

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
                node.transform.localPosition = researchTreeCraft.crafts[0].Position;
                node.Initalize(researchTree.title, researchTreeCraft, _manager);
                return node;
            }
        }
    }
}