using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CraftSystem.Data.Save
{
    public class CSDefaultCraftSaveData : CSNodeSaveData
    {
        [field: SerializeField] public GameObject CraftPrefab { get; set; }
        [field: SerializeField] public int CraftCost { get; set; }
    }
}
