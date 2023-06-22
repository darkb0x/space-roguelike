using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CraftSystem.ScriptableObjects
{
    using Enumerations;
    using Game.Player.Inventory;

    [CreateAssetMenu(fileName = "Craft", menuName = "Game/New Craft")]
    public class CraftSO : ScriptableObject
    {
        [field: SerializeField] public string CraftName { get; set; }
        [field: SerializeField] public string CraftDescription { get; set; }
        [field: SerializeField] public CSCraftType CraftType { get; set; }
        [field: SerializeField] public GameObject CraftPrefab { get; set; }
        [field: SerializeField] public List<ItemData> ItemsInCraft { get; set; }
    }
}
