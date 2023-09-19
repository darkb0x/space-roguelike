using UnityEngine;

namespace Game.Notifications
{
    using UI.HUD;

    public class NotificationService : IEntryComponent<HUDService>
    {
        private const string NOTIFICATION_VISUAL_PATH = "Prefabs/UI/Elements/Notifications/Notification Item";
        private const float NOTIFICATION_DESTROY_TIME = 1.7f;
        public const string GREEN_COLOR = "#6FB372";
        public const string RED_COLOR = "#B01C48";

        private static NotificationService Instance;
        private static NotificationVisual _notificationVisual;

        public bool NotificationsEnabled => Save.SaveManager.UISaveData.EnableNotifications;

        private Transform _notificationsVisualParent;

        public NotificationService()
        {
            _notificationVisual = Resources.Load<NotificationVisual>(NOTIFICATION_VISUAL_PATH);
        }

        public void Initialize(HUDService hudService)
        {
            Instance = this;
            _notificationsVisualParent = hudService.GetHudElement<NotificationsHUD>(HUDElementID.Notifications).GetChildParentTransform();
        }

        public static void NewNotification(Sprite icon, string title, bool highlight, Color textColor, NotificationStyle style = NotificationStyle.Default)
        {
            if (!Instance.NotificationsEnabled)
                return;

            NotificationVisual visual = Object.Instantiate(_notificationVisual, Instance._notificationsVisualParent);
            visual.Initialize(icon, $"<color=#{ColorUtility.ToHtmlStringRGB(textColor)}>{title}</color>", highlight, NOTIFICATION_DESTROY_TIME, style);
        }
    }
}
