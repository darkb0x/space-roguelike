using System.Collections.Generic;
using UnityEngine;

namespace CraftSystem.ScriptableObjects
{
    using Data;
    using Enumerations;
    using Game.Inventory;

    public class CSTreeCraftSO : CraftSO
    {
        [field: SerializeField] public int CraftCost { get; protected set; }
        [field: SerializeField] public bool IsStartingCraft { get; protected set; }
        [field: SerializeField] public bool IsStartingCraftInGroup { get; protected set; }
        [field: SerializeField] public List<CSNextCraftData> Choices { get; protected set; }
        [field: SerializeField] public CSCraftGroupSO Group { get; protected set; }
        [field: SerializeField] public CSCraftContainerSO Container { get; protected set; }
        [field: SerializeField] public Vector2 Position { get; protected set; }

        public void Initialize(string craftName, string craftDescription, int craftCost,
            CSCraftType craftType, Sprite craftIcon, GameObject craftPrefab, List<ItemData> itemsInCraft, 
            bool isStartingCraft, bool isStartingCraftInGroup, List<CSNextCraftData> choices, CSCraftGroupSO group, CSCraftContainerSO container, Vector2 position)
        {
            CraftName = craftName;
            CraftDescription = craftDescription;
            CraftCost = craftCost;
            CraftType = craftType;
            CraftIcon = craftIcon;
            CraftPrefab = craftPrefab;
            ItemsInCraft = itemsInCraft;
            IsStartingCraft = isStartingCraft;
            IsStartingCraftInGroup = isStartingCraftInGroup;
            Choices = choices;
            Group = group;
            Container = container;
            Position = position;
        }
    }
}