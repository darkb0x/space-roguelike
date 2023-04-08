using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Drone
{
    using Player;

    public abstract class DroneAI : MonoBehaviour
    {
        protected PlayerDronesController PlayerDronesController;
        protected bool IsInitialized;
        protected float moveSpeed = 4f;

        public virtual void Initialize(PlayerDronesController pdc)
        {
            pdc.AttachDrone(this);

            IsInitialized = true;
        }
        public void Initialize()
        {
            if(PlayerDronesController != null)
            {
                Initialize(PlayerDronesController);
            }
        }

        public virtual void RotationUpdate(Transform point, float direction, float rangeFromPoint)
        {
            Vector2 targetPos = (Vector2)point.position + new Vector2(Mathf.Sin(direction * Mathf.Deg2Rad), Mathf.Cos(direction * Mathf.Deg2Rad)) * rangeFromPoint;

            transform.position = Vector2.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if(!IsInitialized)
            {
                if (collision.TryGetComponent<PlayerDronesController>(out PlayerDronesController playerDronesController))
                {
                    PlayerDronesController = playerDronesController;
                    Initialize();
                }
            }
        }
    }
}
