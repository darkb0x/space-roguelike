using UnityEngine;

namespace Game.UI.HUD
{
    public class VidgetsHUD : HUDElement
    {
        [SerializeField] private HUDElement[] ChildHUDElements;

        public override HUDElementID ID => HUDElementID.Vidgets;

        public override void Initialize()
        {
            foreach (var child in ChildHUDElements)
                child.Initialize();

            base.Initialize();
        }
    }
}
