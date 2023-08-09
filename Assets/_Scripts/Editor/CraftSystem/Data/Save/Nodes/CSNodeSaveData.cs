using System;
using System.Collections.Generic;
using UnityEngine;

namespace CraftSystem.Data.Save
{
    using Enumerations;

    [Serializable]
    public class CSNodeSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public List<string> OutputIDs { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public List<CSChoiceSaveData> Choices { get; set; }
        [field: SerializeField] public string GroupID { get; set; }
        [field: SerializeField] public CSCraftType CraftType { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}