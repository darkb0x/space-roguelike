using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Editor.ScriptableObjects
{
    public class CSCraftContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SerializableDictionary<CSCraftGroupSO, List<CSCraftSO>> DialogueGroups { get; set; }
        [field: SerializeField] public List<CSCraftSO> UngroupedDialogues { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            DialogueGroups = new SerializableDictionary<CSCraftGroupSO, List<CSCraftSO>>();
            UngroupedDialogues = new List<CSCraftSO>();
        }
    }
}
