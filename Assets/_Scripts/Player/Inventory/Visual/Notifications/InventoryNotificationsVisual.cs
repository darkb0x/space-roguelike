using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Inventory.Visual
{
    public class InventoryNotificationsVisual : MonoBehaviour
    {
        [SerializeField] private Transform NotificationsVisualParent;
        [SerializeField] private InventoryNotificationsVisualItem NotificationVisual;
        [Space]
        [SerializeField] private float NotificationsDestroyTime = 3f;

        public InventoryNotificationsVisualItem NewInventoryNotification(InventoryItem item, int amount, bool isNew, bool isTake)
        {
            InventoryNotificationsVisualItem notification = Instantiate(NotificationVisual.gameObject, NotificationsVisualParent).GetComponent<InventoryNotificationsVisualItem>();
            notification.Initialize(item, amount, isNew, NotificationsDestroyTime, isTake);

            return notification;
        }
    }
}
