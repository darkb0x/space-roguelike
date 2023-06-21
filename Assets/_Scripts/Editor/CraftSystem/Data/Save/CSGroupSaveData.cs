using System;
using UnityEngine;

namespace CraftSystem.Data.Save
{
    [Serializable]
    public class CSGroupSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}