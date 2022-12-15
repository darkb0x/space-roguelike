using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    using Player;
    using Turret;

    public class Workbanch : MonoBehaviour, IMouseObserver_Click
    {
        CSManager craftSystem;
        PlayerController player;
        Transform myTransform;

        private void Start()
        {
            craftSystem = FindObjectOfType<CSManager>();
            player = FindObjectOfType<PlayerController>();
            myTransform = transform;
        }

        public void Craft(GameObject obj)
        {
            GameObject craftedObj = Instantiate(obj, myTransform.position, Quaternion.identity);

            if(craftedObj.TryGetComponent<TurretAI>(out TurretAI turret))
            {
                turret.Initialize(player);
            }

            player.EndCrafting();
        }

        public void MauseDown(MouseClickType mouseClickType)
        {
            if(mouseClickType == MouseClickType.Left)
            {
                if (Vector2.Distance(myTransform.position, player.transform.position) > GameDefaultVariables.interact_maxDistanceBetweenPlayerAndInteractObject)
                    return;

                if(!player.pickObjSystem.pickedGameObject)
                {
                    player.StartCrafting(transform.position);
                    craftSystem.OpenMenu(this);
                }
            }
        }
    }
}
