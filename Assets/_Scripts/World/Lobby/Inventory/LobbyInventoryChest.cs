using System.Collections;
using UnityEngine;

namespace Game.Lobby.Inventory
{
    using Visual;
    using UI;

    public class LobbyInventoryChest : MonoBehaviour
    {
        private readonly int _animIsOpenedBool = Animator.StringToHash("isOpened");

        [SerializeField] private Animator Anim;

        private UIWindowService _uiWindowService;
        private LobbyInventoryVisual _lobbyInventoryWindow;

        public void Initialize(UIWindowService windowService, LobbyInventoryVisual inventoryWindow)
        {
            _uiWindowService = windowService;
            _lobbyInventoryWindow = inventoryWindow;
            _lobbyInventoryWindow.OnOpened += _ => SetVisualEnabled(true);
            _lobbyInventoryWindow.OnClosed += _ => SetVisualEnabled(false);
        }

        public void SetVisualOpened()
        {
            SetVisualEnabled(true);
        }
        public void SetVisualClosed()
        {
            SetVisualEnabled(false);
        }
        private void SetVisualEnabled(bool enabled)
        {
            Anim.SetBool(_animIsOpenedBool, enabled);
        }

        private void Open()
        {
            _uiWindowService.TryOpenWindow(LobbyInventory.LOBBY_INVENTORY_WINDOW_ID);
        }
    }
}