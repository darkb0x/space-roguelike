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
                if (Vector2.Distance(myTransform.position, player.transform.position) > GameDefaultVariables.interact_maxDistanceBetweenPlayerAndInteractObject)
                    return;

                learnSystem.OpenMenu();
            }
        }
    }
}
