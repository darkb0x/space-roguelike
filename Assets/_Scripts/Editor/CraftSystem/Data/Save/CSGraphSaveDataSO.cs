using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Editor.Data.Save
{
    public class CSGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<CSNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            Nodes = new List<CSNodeSaveData>();
        }
    }
}
