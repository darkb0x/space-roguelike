using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace Game.MainMenu
{
    using SceneLoading;
    using Audio;

    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private AudioClip Music;
        [Space]
        [SerializeField, Scene] private int LobbySceneId;
        [Space]
        [SerializeField] private string YTChanelURL;
        [SerializeField] private string DiscordURL;

        private void Awake()
        {
            PlayerPrefs.SetInt("LoadingSceen_used", 1);
            PlayerPrefs.SetInt("LoadingSceen_currentFrame", 0);

            Time.timeScale = 1f;

            LogUtility.StopLogging();
        }

        private void Start()
        {
            MusicManager.Instance.SetMusic(Music, true);
        }

        public void PlayButton()
        {
            LogUtility.StartLogging("session");

            SaveData.SaveDataManager.Instance.CurrentSessionData.Reset();

            LoadSceneUtility.LoadSceneAsyncVisualize(LobbySceneId);
        }

        public void OpenYoutubeChanel()
        {
            Application.OpenURL(YTChanelURL);
        }
        public void OpenDiscrordServer()
        {
            Application.OpenURL(DiscordURL);
        }
    }
}
