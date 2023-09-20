using UnityEngine;

namespace Game.Menu.Pause.Exit
{
    using SceneLoading;
    using Save;
    using UI;

    public class ExitManager : MonoBehaviour, IEntryComponent<UIWindowService>
    {
        public const WindowID EXIT_WINDOW_ID = WindowID.ExitFromGame;

        [NaughtyAttributes.Scene] public int MenuSceneID;

        private UIWindowService _uiWindowService;

        public void Initialize(UIWindowService uiWindowService)
        {
            _uiWindowService = uiWindowService;

            _uiWindowService.RegisterWindow<ExitWindow>(EXIT_WINDOW_ID)
                .Initialize(this);
        }

        public void OpenMenu()
        {
            SaveManager.SessionSaveData.Reset();
            LoadSceneUtility.LoadSceneAsyncVisualize(MenuSceneID);
        }

        public void Exit()
        {
            SaveManager.SessionSaveData.Reset();
            Application.Quit();
        }
    }
}
