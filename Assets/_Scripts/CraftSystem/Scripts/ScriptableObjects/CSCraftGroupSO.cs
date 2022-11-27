using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Editor.ScriptableObjects
{
    public class CSCraftGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }

        public void Initialize(string groupName)
        {
            GroupName = groupName;
        }
    }
}
