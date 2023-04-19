using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Editor.ScriptableObjects
{
    public class CSCraftContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<CSCraftSOTree> Nodes { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            Nodes = new List<CSCraftSOTree>();
        }
    }
}
