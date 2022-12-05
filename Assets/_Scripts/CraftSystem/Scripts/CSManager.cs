using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.CraftSystem
{
    using CraftSystem.Editor.ScriptableObjects;
    using Player.Inventory;

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
        public List<CSCraftUIObject> loadedCraftPrefabs;

        public Transform techTreeRenderTransform;
        public Transform techTreeArrowRenderTransform;
    }

    public class CSManager : MonoBehaviour
    {
        [Header("Tech Tree")]
        [SerializeField] private List<TechTree> techTrees = new List<TechTree>();

        [Header("UI/Prefabs")]
        [SerializeField] private CSCraftUIObject craftPrefab;
        [SerializeField] private CSCraftUIArrow arrowPrefab;

        [Header("Other")]
        [SerializeField] private Canvas canvas;

        public void Start()
        {
            SpawnNodes();
        }

        [NaughtyAttributes.Button]
        public void SpawnNodes()
        {
            foreach (TechTree tree in techTrees)
            {
                tree.loadedCraftPrefabs = new List<CSCraftUIObject>();

                SpawnNodes(tree);
                SpawnConnections(tree);
            }
        }

        public void Craft(CSCraftSO craft)
        {

        }

        #region Utilities
        public void SpawnNodes(TechTree tree)
        {
            if(tree.techTreeRenderTransform.childCount > 0)
            {
                int childCount = tree.techTreeRenderTransform.childCount;
                for (int i = childCount - 1; i > 0; i--)
                {
                    DestroyImmediate(tree.techTreeRenderTransform.GetChild(i).gameObject);
                }
                tree.loadedCraftPrefabs.Clear();
            }

            foreach (CSCraftSO craftData in tree.techTree.UngroupedDialogues)
            {
                CSCraftUIObject obj = Instantiate(craftPrefab.gameObject, tree.techTreeRenderTransform).GetComponent<CSCraftUIObject>();
                obj.Initialize(craftData, new Vector2(craftData.Position.x, -craftData.Position.y));
                tree.loadedCraftPrefabs.Add(obj);
            }
        }
        public void SpawnConnections(TechTree tree)
        {
            if (tree.techTreeArrowRenderTransform.childCount > 0)
            {
                int childCount = tree.techTreeArrowRenderTransform.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(tree.techTreeArrowRenderTransform.GetChild(i).gameObject);
                }
            }

            foreach (CSCraftUIObject craftUIObject in tree.loadedCraftPrefabs)
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
        
        public CSCraftUIObject GetCraftObj(CSCraftSO so)
        {
            foreach (CSCraftUIObject uiObj in GetLoadedNodesByNodeSO(so))
            {
                if(uiObj.craft == so)
                {
                    return uiObj;
                }
            }
            return null;
        }
        public List<CSCraftUIObject> GetCraftObjInChoices(CSCraftSO so)
        {
            List<CSCraftUIObject> objectList = new List<CSCraftUIObject>();

            foreach (CSCraftUIObject uiObj in GetLoadedNodesByNodeSO(so))
            {
                if(uiObj.craft.Choices.Count > 0 && uiObj.craft.Choices != null)
                {
                    foreach (var choice in uiObj.craft.Choices)
                    {
                        if(choice.NextCraft == so)
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

        public List<CSCraftUIObject> GetLoadedNodesByNodeSO(CSCraftSO nodeSO)
        {
            foreach (TechTree tree in techTrees)
            {
                foreach (CSCraftUIObject item in tree.loadedCraftPrefabs)
                {
                    if(item.craft == nodeSO)
                    {
                        return tree.loadedCraftPrefabs;
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
