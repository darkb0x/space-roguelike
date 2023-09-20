using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory
{
    using UI.HUD;

    public class InventoryButtonHUD : HUDElement
    {
        [SerializeField] private Button Button;

        public override HUDElementID ID => HUDElementID.InventoryButton;

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();

            var inventoryHUD = GetHudElement<InventoryHUD>(HUDElementID.Inventory);
            Button.onClick.AddListener(inventoryHUD.OpenClose);
        }
    }
}
