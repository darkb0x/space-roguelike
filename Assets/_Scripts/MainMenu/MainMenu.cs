using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace Game.MainMenu
{
    using Utilities.LoadScene;
    using Utilities;
    using SaveData;

    public class MainMenu : MonoBehaviour
    {
        [SerializeField, Scene] private int LobbySceneId;
        [Space]
        [SerializeField] private string YTChanelURL;
        [SerializeField] private string DiscordURL;

        private void Awake()
        {
            PlayerPrefs.SetInt("LoadingSceen_used", 1);
            PlayerPrefs.SetInt("LoadingSceen_currentFrame", 0);

            Time.timeScale = 1f;
        }

        private void Start()
        {
            LogUtility.StopLogging();

            GameData.Instance.ResetSessionData();
        }

        public void PlayButton()
        {
            if(GameData.Instance.CurrentSettingsData.EnableLogs)
            {
                LogUtility.StartLogging("session");
            }

            LoadSceneUtility.Instance.LoadSceneAsyncVisualize(LobbySceneId);
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
