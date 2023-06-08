using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Lobby
{
    using Audio;

    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private AudioClip Music;

        private void Start()
        {
            MusicManager.Instance.SetMusic(Music, true);
        }
    }
}
