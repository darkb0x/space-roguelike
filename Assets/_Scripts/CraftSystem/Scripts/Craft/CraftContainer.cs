using UnityEngine;
using System.Collections.Generic;

namespace Game.CraftSystem.Craft
{
    using Visual;
    using global::CraftSystem.ScriptableObjects;
    using System.Linq;

    [System.Serializable]
    public class CraftContainer
    {
        [SerializeField] private bool m_Enabled;
        [Space]
        [SerializeField] private string m_Title;
        [SerializeField] private Sprite m_Icon;

        private RectTransform m_VisualParent;
        private List<CraftSO> m_Crafts = new List<CraftSO>();
        private List<CraftNodeVisual> m_CraftsVisual = new List<CraftNodeVisual>();

        public bool Enabled => m_Enabled;
        public string Title => m_Title;
        public Sprite Icon => m_Icon;
        public RectTransform VisualParent => m_VisualParent;

        public void InjectVisual(RectTransform visualParent)
        {
            m_VisualParent = visualParent;
        }

        public void AddCraft(CraftSO craft)
        {
            m_Crafts.Add(craft);
        }
        public void AddCraftVisual(CraftNodeVisual nodeVisual)
        {
            m_CraftsVisual.Add(nodeVisual);
        }

        public CraftSO[] GetCrafts()
        {
            m_Crafts.OrderBy(x => x.CraftName);
            return m_Crafts.ToArray();
        }
        public CraftNodeVisual[] GetCraftVisuals()
        {
            return m_CraftsVisual.ToArray();
        }
    }
}
