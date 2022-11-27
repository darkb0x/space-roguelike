using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.CraftSystem.Editor.Data
{
    using ScriptableObjects;

    [Serializable]
    public class CSCraftChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public CSCraftSO NextDialogue { get; set; }
    }
}
