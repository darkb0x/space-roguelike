using System.Collections.Generic;
using CraftSystem.ScriptableObjects;
using UnityEngine;

namespace Game.CraftSystem
{
    [System.Serializable]
    public class ResearchTree
    {
        public struct ClampedPosition
        {
            public Vector2 Min;
            public Vector2 Max;
        }

        [SerializeField] private bool m_Enabled;
        [Space]
        [SerializeField] private string m_Title;
        [SerializeField] private Sprite m_Icon;
        [Space]
        [SerializeField] private CSCraftContainerSO m_CraftTree;

        private Transform _mainVisualParent;
        private Transform _connectionsVisualParent;
        private ClampedPosition _clampedPosition;

        public bool Enabled => m_Enabled;
        public string Title => m_Title;
        public Sprite Icon => m_Icon;
        public CSCraftContainerSO CraftTree => m_CraftTree;
        public Transform MainVisualParent => _mainVisualParent;
        public Transform ConnectionsVisualParent => _connectionsVisualParent;
        public ClampedPosition ClampedVisualPosition => _clampedPosition;

        public void InjectVisual(List<Visual.Node.CraftTreeNodeVisual> nodes, Transform mainVisualParent, Transform connectionsVisualParent)
        {
            _mainVisualParent = mainVisualParent;
            _connectionsVisualParent = connectionsVisualParent;

            Vector2 spacing = new Vector2(100, 0);
            Vector2 min = (Vector2)nodes[0].transform.localPosition;
            Vector2 max = (Vector2)nodes[nodes.Count - 1].transform.localPosition;
            foreach (var node in nodes)
            {
                var nodePosition = node.transform.localPosition;

                // min
                if(nodePosition.x < min.x)
                {
                    min = new Vector2(nodePosition.x, min.y);
                }
                if(nodePosition.y < min.y)
                {
                    min = new Vector2(min.x, nodePosition.y);
                }

                // max
                if(nodePosition.x > max.x)
                {
                    max = new Vector2(nodePosition.x, max.y);
                }
                if(nodePosition.y > max.y)
                {
                    max = new Vector2(max.x, nodePosition.y);
                }
            }

            _clampedPosition = new ClampedPosition()
            {
                Min = min - spacing,
                Max = max + spacing
            };
        }
    }
}
