using UnityEngine;

namespace Game.Notifications
{
    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] protected Transform NotificationsVisualParent;
        [SerializeField] protected NotificationVisual NotificationVisual;
        [Space]
        [SerializeField] private float NotificationsDestroyTime = 3f;

        private static NotificationManager Instance;

        public static string GreenColor = "#6FB372";
        public static string RedColor = "#B01C48";

        public bool NotificationsEnabled => Save.SaveManager.UISaveData.EnableNotifications;

        private void Awake()
        {
            Instance = this;
        }

        public static void NewNotification(Sprite icon, string title, bool highlight, Color textColor, NotificationStyle style = NotificationStyle.Default)
        {
            if (!Instance.NotificationsEnabled)
                return;

            NotificationVisual visual = Instantiate(Instance.NotificationVisual.gameObject, Instance.NotificationsVisualParent).GetComponent<NotificationVisual>();
            visual.Initialize(icon, $"<color=#{ColorUtility.ToHtmlStringRGB(textColor)}>{title}</color>", highlight, Instance.NotificationsDestroyTime, style);
        }
    }
}
