using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.CraftSystem.Editor.Data.Save
{
    [Serializable]
    public class CSGroupSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }

    }
}
