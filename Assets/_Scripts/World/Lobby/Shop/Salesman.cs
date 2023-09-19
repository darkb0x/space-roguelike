using System.Collections;
using UnityEngine;

namespace Game.Lobby.Shop
{
    using UI;

    public class Salesman : MonoBehaviour
    {
        private UIWindowService _uiWindowService;
        private void Start()
        {
            _uiWindowService = ServiceLocator.GetService<UIWindowService>();
        }
        public void Open()
        {
            _uiWindowService.TryOpenWindow(ShopManager.SHOP_WINDOW_ID);
        }
    }
}