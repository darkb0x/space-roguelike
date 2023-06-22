using System.Collections.Generic;
using UnityEngine;

namespace CraftSystem.ScriptableObjects
{
    using Data;
    using Enumerations;
    using Game.Player.Inventory;

    public class CSTreeCraftSO : CraftSO
    {
        [field: SerializeField] public int CraftCost { get; set; }
        [field: SerializeField] public bool IsStartingCraft { get; set; }
        [field: SerializeField] public List<CSNextCraftData> Choices { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }

        public void Initialize(string craftName, string craftDescription, int craftCost,
            CSCraftType craftType, GameObject craftPrefab, List<ItemData> itemsInCraft, 
            bool isStartingCraft, List<CSNextCraftData> choices, Vector2 position)
        {
            CraftName = craftName;
            CraftDescription = craftDescription;
            CraftCost = craftCost;
            CraftType = craftType;
            CraftPrefab = craftPrefab;
            ItemsInCraft = itemsInCraft;
            IsStartingCraft = isStartingCraft;
            Choices = choices;
            Position = position;
        }
    }
}