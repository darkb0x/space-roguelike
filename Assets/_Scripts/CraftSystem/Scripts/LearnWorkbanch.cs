using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    using Player;

    public class LearnWorkbanch : MonoBehaviour, IMouseObserver_Click
    {
        LearnCSManager learnSystem;
        PlayerController player;
        Transform myTransform;

        [SerializeField] private float radius;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private void Start()
        {
            learnSystem = FindObjectOfType<LearnCSManager>();
            player = FindObjectOfType<PlayerController>();
            myTransform = transform;
        }

        public void MauseDown(MouseClickType mouseClickType)
        {
            if(mouseClickType == MouseClickType.Left)
            {
                if (Vector2.Distance(myTransform.position, player.transform.position) > radius)
                    return;

                learnSystem.OpenMenu();
            }
        }
    }
}
