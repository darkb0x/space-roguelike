using UnityEngine;

namespace Game.Inventory
{
    using UI.HUD;
    using Input;

    public class InventoryHUD : HUDElement
    {
        [SerializeField] private InventoryVisual InventoryVisual;

        public override HUDElementID ID => HUDElementID.Inventory;

        private PlayerInputHandler _input => InputManager.PlayerInputHandler;

        public override void Initialize()
        {
            InventoryVisual.Initialize();
            base.Initialize();

            Hide();
        }

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            _input.InventoryEvent += OpenClose;
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            _input.InventoryEvent -= OpenClose;
        }

        public void OpenClose()
        {
            if (View.gameObject.activeSelf)
                Hide();
            else
                Show();
        }
    }
}
