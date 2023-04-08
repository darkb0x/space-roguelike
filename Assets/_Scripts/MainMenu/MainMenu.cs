using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace Game.MainMenu
{
    using Utilities.LoadScene;
    using SaveData;

    public class MainMenu : MonoBehaviour
    {
        [SerializeField, Scene] private int LobbySceneId;
        [Space]
        [SerializeField] private string YTChanelURL;
        [SerializeField] private string DiscordURL;

        private void Awake()
        {
            GameData.Instance.ResetSessionData();

            PlayerPrefs.SetInt("LoadingSceen_used", 1);
            PlayerPrefs.SetInt("LoadingSceen_currentFrame", 0);

            Time.timeScale = 1f;
        }

        public void PlayButton()
        {
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
