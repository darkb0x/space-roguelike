using System.Collections.Generic;
using UnityEngine;

namespace CraftSystem.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class CSTreeCraftSO : CraftSO
    {
        [field: SerializeField] public List<CSNextCraftData> Choices { get; set; }
        [field: SerializeField] public bool IsStartingCraft { get; set; }

        public void Initialize(string craftName, string craftDescription, List<CSNextCraftData> choices, CSCraftType craftType, GameObject craftPrefab, bool isStartingCraft)
        {
            CraftName = craftName;
            CraftDescription = craftDescription;
            Choices = choices;
            CraftType = craftType;
            CraftPrefab = craftPrefab;
            IsStartingCraft = isStartingCraft;
        }
    }
}