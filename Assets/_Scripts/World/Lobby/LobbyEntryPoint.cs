using System.Collections;
using UnityEngine;

namespace Game.Lobby
{
    using Player;

    public class LobbyEntryPoint : MonoBehaviour
    {
        [SerializeField] private PlayerController Player;

        void Start()
        {
            Player.Oxygen.Disable();
        }
    }
}