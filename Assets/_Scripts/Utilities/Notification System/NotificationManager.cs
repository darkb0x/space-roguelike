using System.Collections;
using System.Collections.Generic;
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

        public static string GreenColor = "#42D189";
        public static string RedColor = "#B01C48";

        private void Awake()
        {
            Instance = this;
        }

        public static void NewNotification(Sprite icon, string title, bool highlight)
        {
            NotificationVisual visual = Instantiate(Instance.NotificationVisual.gameObject, Instance.NotificationsVisualParent).GetComponent<NotificationVisual>();
            visual.Initialize(icon, title, highlight, Instance.NotificationsDestroyTime);
        }
    }
}
