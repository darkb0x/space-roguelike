using Game.Player.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CraftSystem.Data.Save
{
    [Serializable]
    public class CSDefaultCraftSaveData : CSNodeSaveData
    {
        [field: SerializeField] public Sprite CraftIcon;
        [field: SerializeField] public GameObject CraftPrefab { get; set; }
        [field: SerializeField] public int CraftCost { get; set; }
        [field: SerializeField] public List<ItemData> ItemsInCraft { get; set; }
    }
}
