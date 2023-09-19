using UnityEngine;

namespace Game.CraftSystem.Craft
{
    using UI;

    public class CraftTable : MonoBehaviour
    {
        private UIWindowService _uiWindowService;

        private void Start()
        {
            _uiWindowService = ServiceLocator.GetService<UIWindowService>();
        }

        public void OpenCraftPanel()
        {
            _uiWindowService.TryOpenWindow(CraftManager.CRAFT_WINDOW_ID);
        }
    }
}