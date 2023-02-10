using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.CraftSystem.Editor.Data.Save
{
    using ScriptableObjects;

    [Serializable]
    public class CSNodeSaveData
    {
        [field: SerializeField] public string NodeID { get; set; }
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public List<CSChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public GameObject ObjectPrefab { get; set; }
        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public int Cost { get; set; }
        [field: SerializeField] public List<ItemCraft> Craft { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}
