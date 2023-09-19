using UnityEngine;

namespace Game.Notifications
{
    using UI.HUD;

    public class NotificationsHUD : HUDElement
    {
        [SerializeField] private Transform NotificationsVisualParent;

        public override HUDElementID ID => HUDElementID.Notifications;

        public Transform GetChildParentTransform()
        {
            return NotificationsVisualParent;
        }
    }
}
