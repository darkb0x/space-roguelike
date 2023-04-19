using UnityEngine;

namespace Game.CraftSystem
{
    using Player;
    using Drill;
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

            if(craftedObj.TryGetComponent(out Turret turret))
            {
                turret.Initialize(player);
            }
            else if(craftedObj.TryGetComponent(out DroneAI drone))
            {
                drone.Initialize(player.GetComponent<PlayerDronesController>());
            }
            else if(craftedObj.TryGetComponent(out Drill drill))
            {
                drill.Initialize();
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
