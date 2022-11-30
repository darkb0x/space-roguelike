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
    }

    public class CSManager : MonoBehaviour
    {
        [Header("Tech Tree")]
        [SerializeField] private List<TechTree> techTrees = new List<TechTree>();

        [Header("UI/Prefabs")]
        [SerializeField] private CSCraftUIObject craftPrefab;

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
                for (int i = childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(tree.techTreeRenderTransform.GetChild(i).gameObject);
                }
                tree.loadedCraftPrefabs.Clear();
            }

            foreach (CSCraftSO craftData in tree.techTree.UngroupedDialogues)
            {
                CSCraftUIObject obj = Instantiate(craftPrefab.gameObject, tree.techTreeRenderTransform).GetComponent<CSCraftUIObject>();
                obj.Initialize(craftData, new Vector2(craftData.Position.x, -craftData.Position.y), this);
                tree.loadedCraftPrefabs.Add(obj);
            }
        }

        
        public CSCraftUIObject GetCraftObj(CSCraftSO so)
        {
            foreach (CSCraftUIObject uiObj in GetLoadedNodesByNodeSO(so))
            {
                if(uiObj.node == so)
                {
                    return uiObj;
                }
            }
            return null;
        }

        public List<CSCraftUIObject> GetLoadedNodesByNodeSO(CSCraftSO nodeSO)
        {
            foreach (TechTree tree in techTrees)
            {
                foreach (CSCraftUIObject item in tree.loadedCraftPrefabs)
                {
                    if(item.node == nodeSO)
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
