using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace Game.Menu
{
    using SceneLoading;
    using Audio;
    using UI;
    using Game.Menu.Settings;
    using Game.Menu.Pause.Exit;

    public class MainMenu : MonoBehaviour, IEntryComponent<UIWindowService>, IService
    {
        private const string YT_CHANEL_LINK = "";
        private const string DISCORD_SERVER_LINK = "";

        [SerializeField] private Button StartButton;
        [SerializeField] private Button SettingsButton;
        [SerializeField] private Button ExitButton;
        [Space]
        [SerializeField] private Button YTChanelButton;
        [SerializeField] private Button DiscordButton;
        [Space]
        [SerializeField] private AudioClip Music;
        [Space]
        [SerializeField, Scene] private int LobbySceneId;

        private UIWindowService _uiWindowService;

        public void Initialize(UIWindowService windowService)
        {
            _uiWindowService = windowService;

            InitButtons();

            PlayerPrefs.SetInt("LoadingSceen_used", 1);
            PlayerPrefs.SetInt("LoadingSceen_currentFrame", 0);

            Time.timeScale = 1f;

            LogUtility.StopLogging();

            MusicManager.Instance.SetMusic(Music, true);

        }

        private void InitButtons()
        {
            StartButton.onClick.AddListener(StartGame);
            SettingsButton.onClick.AddListener(() => _uiWindowService.TryOpenWindow(SettingsManager.SETTINGS_WINDOW_ID));
            ExitButton.onClick.AddListener(() => _uiWindowService.TryOpenWindow(ExitManager.EXIT_WINDOW_ID));

            YTChanelButton.onClick.AddListener(OpenYoutubeChanel);
            DiscordButton.onClick.AddListener(OpenDiscrordServer);
        }

        public void StartGame()
        {
            LogUtility.StartLogging("session");

            Save.SaveManager.SessionSaveData.Reset();

            LoadSceneUtility.LoadSceneAsyncVisualize(LobbySceneId);
        }

        public void OpenYoutubeChanel()
        {
            Application.OpenURL(YT_CHANEL_LINK);
        }
        public void OpenDiscrordServer()
        {
            Application.OpenURL(DISCORD_SERVER_LINK);
        }
    }
}
