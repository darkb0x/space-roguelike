using UnityEngine;

namespace Game.Menu.Pause.Exit
{
    using SceneLoading;
    using Save;
    using UI;

    public class ExitManager : MonoBehaviour, IEntryComponent<UIWindowService>
    {
        public const WindowID EXIT_WINDOW_ID = WindowID.ExitFromGame;

        [SerializeField, NaughtyAttributes.Scene] private int MenuSceneID;

        public void Initialize(UIWindowService uiWindowService)
        {
            uiWindowService.RegisterWindow<ExitWindow>(EXIT_WINDOW_ID);
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
