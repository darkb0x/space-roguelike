using CraftSystem.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    [System.Serializable]
    public class ResearchTree
    {
        [SerializeField] private string m_Title;
        [SerializeField] private Sprite m_Icon;
        [Space]
        [SerializeField] private CSCraftContainerSO m_CraftTree;

        private Transform _mainVisualParent;
        private Transform _connectionsVisualParent;

        public string Title => m_Title;
        public Sprite Icon => m_Icon;
        public CSCraftContainerSO CraftTree => m_CraftTree;
        public Transform MainVisualParent => _mainVisualParent;
        public Transform ConnectionsVisualParent => _connectionsVisualParent;

        public void InjectVisual(Transform mainVisualParent, Transform connectionsVisualParent)
        {
            _mainVisualParent = mainVisualParent;
            _connectionsVisualParent = connectionsVisualParent;
        }
    }
}
