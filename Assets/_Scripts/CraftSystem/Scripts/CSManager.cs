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

    public class CSManager : MonoBehaviour
    {
        [Header("Tech Tree")]
        public CSCraftContainerSO techTree;
        [SerializeField] private CSCraftUIObject[] loadedCraftPrefabs;

        [Header("UI")]
        [SerializeField] private Transform techTreeRenderTransform;

        [Header("UI/Prefabs")]
        [SerializeField] private CSCraftUIObject craftPrefab;

        public void Start()
        {
            
        }

        [NaughtyAttributes.Button]
        public void Spawn()
        {
            SpawnNodes(techTree, techTreeRenderTransform);
        }

        public void SpawnNodes(CSCraftContainerSO techTree, Transform renderTransform)
        {
            foreach (CSCraftSO craftData in techTree.UngroupedDialogues)
            {
                CSCraftUIObject obj = Instantiate(craftPrefab.gameObject, renderTransform).GetComponent<CSCraftUIObject>();
                obj.Initialize(craftData, new Vector2(craftData.Position.x, -craftData.Position.y), this);
            }
        }

        public CSCraftUIObject GetCraftObj(CSCraftSO so)
        {
            foreach (CSCraftUIObject uiObj in loadedCraftPrefabs)
            {
                if(uiObj.node == so)
                {
                    return uiObj;
                }
            }
            return null;
        }
    }
}
