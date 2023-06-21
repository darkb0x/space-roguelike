using System;
using UnityEngine;

namespace CraftSystem.Data.Save
{
    [Serializable]
    public class CSChoiceSaveData
    {
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public string NodeID { get; set; }
    }
}