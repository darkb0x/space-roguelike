using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory
{
    using Game.Player;
    using UI.HUD;

    public class InventoryButtonHUD : HUDElement
    {
        [SerializeField] private Button Button;

        public override HUDElementID ID => HUDElementID.InventoryButton;

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();

            var playerInventory = ServiceLocator.GetService<PlayerInventory>();
            Button.onClick.AddListener(playerInventory.OpenClose);
        }
    }
}
