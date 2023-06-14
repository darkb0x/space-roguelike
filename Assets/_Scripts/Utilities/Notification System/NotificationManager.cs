using UnityEngine;

namespace Game.Utilities.Notifications
{
    using Visual;

    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] protected Transform NotificationsVisualParent;
        [SerializeField] protected NotificationVisual NotificationVisual;
        [Space]
        [SerializeField] private float NotificationsDestroyTime = 3f;

        private static NotificationManager Instance;

        public static string GreenColor = "#6FB372";
        public static string RedColor = "#B01C48";

        private void Awake()
        {
            Instance = this;
        }

        public static void NewNotification(Sprite icon, string title, bool highlight, NotificationStyle style = NotificationStyle.Default, string textColor = "ffffff")
        {
            NotificationVisual visual = Instantiate(Instance.NotificationVisual.gameObject, Instance.NotificationsVisualParent).GetComponent<NotificationVisual>();
            visual.Initialize(icon, $"<color=#{textColor}>{title}</color>", highlight, Instance.NotificationsDestroyTime, style);
        }
    }
}
