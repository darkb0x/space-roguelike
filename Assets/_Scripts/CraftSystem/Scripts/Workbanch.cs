using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    using Player;
    using Turret;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class Workbanch : MonoBehaviour
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

            if(craftedObj.TryGetComponent<Turret>(out Turret turret))
            {
                //turret.Initialize(player);
            }

            player.ContinuePlayerMove();
        }

        public void OpenCraftMenu()
        {
            if (!player.pickObjSystem.pickedGameObject)
            {
                player.StopPlayerMove(transform.position);
                craftSystem.OpenMenu(this);
            }
        }
    }
}
