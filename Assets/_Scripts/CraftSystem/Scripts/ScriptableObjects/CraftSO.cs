using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CraftSystem.ScriptableObjects
{
    using Data;
    using Enumerations;

    [CreateAssetMenu(fileName = "Craft", menuName = "Game/New Craft")]
    public class CraftSO : ScriptableObject
    {
        [field: SerializeField] public string CraftName { get; set; }
        [field: SerializeField] public string CraftDescription { get; set; }
        [field: SerializeField] public CSCraftType CraftType { get; set; }
    }
}
