using System.Collections;
using UnityEngine;

namespace Game.Lobby.Missions
{
    using UI;

    public class MissionTable : MonoBehaviour
    {
        private UIWindowService _uiWindowService;

        private void Start()
        {
            _uiWindowService = ServiceLocator.GetService<UIWindowService>();
        }

        public void Open()
        {
            _uiWindowService.TryOpenWindow(MissionChooseManager.MISSION_SCREEN_WINDOW_ID);
        }
    }
}