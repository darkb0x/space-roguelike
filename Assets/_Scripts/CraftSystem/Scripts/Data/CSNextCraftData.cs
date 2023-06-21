using System;
using UnityEngine;

namespace CraftSystem.Data
{
    using ScriptableObjects;

    [Serializable]
    public class CSNextCraftData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public CSTreeCraftSO Next { get; set; }
    }
}