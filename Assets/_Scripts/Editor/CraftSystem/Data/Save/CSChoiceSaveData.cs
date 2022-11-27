using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Game.CraftSystem.Editor.Data.Save
{
    [Serializable]
    public class CSChoiceSaveData
    {
        [field: SerializeField] public string NodeID { get; set; }
    }
}
