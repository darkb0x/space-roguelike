using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    using Player;
    using Turret;
    using Drone;

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

        public void Craft(GameObject objPrefab)
        {
            GameObject craftedObj = Instantiate(objPrefab, myTransform.position, Quaternion.identity);

            if(craftedObj.TryGetComponent<Turret>(out Turret turret))
            {
                turret.Initialize(player);
            }
            else if(craftedObj.TryGetComponent<DroneAI>(out DroneAI drone))
            {
                drone.Initialize(player.GetComponent<PlayerDronesController>());
            }

            player.ContinuePlayerMove();
        }

        public void OpenCraftMenu()
        {
            player.StopPlayerMove();
            craftSystem.OpenMenu(this);
        }
    }
}
