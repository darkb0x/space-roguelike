using UnityEngine;

namespace Game.Inventory
{
    using UI;

    public class InventoryWindow : Window
    {
        [SerializeField] private InventoryVisual InventoryVisual;

        public override WindowID ID => WindowID.Inventory;

        public override void Initialize(UIWindowService service)
        {
            base.Initialize(service);
            InventoryVisual.Initialize();
        }

        public void OpenClose()
        {
            if (IsOpened)
                _uiWindowService.Close(ID);
            else
                _uiWindowService.TryOpenWindow(ID);
        }
    }
}
