using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace CraftSystem.Data.Save
{
    public class CSGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<CSGroupSaveData> Groups { get; set; }
        [field: SerializeField, SerializeReference] public List<CSNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldGroupNames { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames { get; set; }
        [field: SerializeField] public SerializedDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            Groups = new List<CSGroupSaveData>();
            Nodes = new List<CSNodeSaveData>();
        }
    }
}