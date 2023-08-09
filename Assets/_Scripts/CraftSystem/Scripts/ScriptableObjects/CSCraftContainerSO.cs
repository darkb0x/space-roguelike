using System.Collections.Generic;
using System.Linq;
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

        public int GetConnectionsCount()
        {
            int result = 0;

            foreach (var group in CraftGroups.Keys)
            {
                result += CraftGroups[group].Last().Choices.Count;
            }

            return result;
        }
        public CSTreeCraftSO GetStartCraft()
        {
            foreach (var craft in UngroupedCrafts)
            {
                if (craft.IsStartingCraft)
                    return craft;
            }

            foreach (var group in CraftGroups.Keys)
            {
                foreach (var craft in CraftGroups[group])
                {
                    if (craft.IsStartingCraft)
                        return craft;
                }
            }

            return null;
        }
        public CSTreeCraftSO GetStartCraftInGroup(CSCraftGroupSO group)
        {
            foreach (var craft in CraftGroups[group])
            {
                if (craft.IsStartingCraftInGroup)
                    return craft;
            }

            return null;
        }
    }
}