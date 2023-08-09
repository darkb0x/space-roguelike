using CraftSystem.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    using Visual.Node;

    [System.Serializable]
    public class ResearchTree
    {
        public string title;
        public Sprite icon;
        [Space]
        public CSCraftContainerSO craftTree;
    }
}
