using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace Game.MainMenu
{
    using Utilities;

    public class MainMenu : MonoBehaviour
    {
        [SerializeField, Scene] private int LobbySceneId;

        public void PlayButton()
        {
            LoadSceneUtility.Instance.LoadScene(LobbySceneId);
        }
    }
}
