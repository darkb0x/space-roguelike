using System.Collections;
using UnityEngine;

namespace Game.Lobby.Inventory
{
    using Visual;

    public class LobbyInventoryChest : MonoBehaviour
    {
        private readonly int _animIsOpenedBool = Animator.StringToHash("isOpened");

        [SerializeField] private Animator Anim;

        private LobbyInventoryVisual _lobbyInventoryVisual;

        private void Start()
        {
            _lobbyInventoryVisual = ServiceLocator.GetService<LobbyInventory>().Visual;
        }

        public void Open()
        {
            _lobbyInventoryVisual.Open(this);
        }

        public void SetVisualOpened(bool enabled)
        {
            Anim.SetBool(_animIsOpenedBool, enabled);
        }
    }
}