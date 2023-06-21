using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace CraftSystem.ScriptableObjects
{
    public class CSCraftContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SerializedDictionary<CSCraftGroupSO, List<CSTreeCraftSO>> CraftGroups { get; set; }
        [field: SerializeField] public List<CSTreeCraftSO> UngroupedCrafts { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            CraftGroups = new SerializedDictionary<CSCraftGroupSO, List<CSTreeCraftSO>>();
            UngroupedCrafts = new List<CSTreeCraftSO>();
        }
    }
}