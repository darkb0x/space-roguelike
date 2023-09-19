using UnityEngine;

namespace Game.CraftSystem.Research
{
    using UI;

    public class ResearchTable : MonoBehaviour
    {
        private UIWindowService _uiWindowService;

        private void Start()
        {
            _uiWindowService = ServiceLocator.GetService<UIWindowService>();
        }

        public void OpenResearchManager()
        {
            _uiWindowService.TryOpenWindow(ResearchManager.CRAFT_RESEARCH_WINDOW_ID);
        }
    }
}