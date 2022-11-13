using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Drone
{
    using Player.Inventory;

    [CreateAssetMenu(fileName = "drone action mine", menuName = "Drone/new action mine")]
    public class DA_mine : DroneAction
    {
        PlayerInventory playerInventory;
        Transform playerTransform;
        Transform myTransform;
        Vector3 moveTarget;
        float mineTime;
        bool doMove = false;

        [SerializeField] private float maxDistance = 7f; // Between drone and player
        [SerializeField] private float moveSpeed = 0.2f;
        [SerializeField] private float mineSpeed = 1.3f;
        [SerializeField] private int itemPerTime = 1;

        public override void Init()
        {
            playerTransform = player.transform;
            myTransform = drone.transform;
            playerInventory = FindObjectOfType<PlayerInventory>();

            drone.playerDrCo.AttachMinerDrone(drone);
            drone.playerDrCo.AttachRotateDrones(drone);
            mineTime = mineSpeed;
        }

        public override void Run()
        {
            Vector3 returnPos = drone.GetReturnPos();
            if (drone.targetOre != null && Vector2.Distance(myTransform.position, playerTransform.position) < maxDistance)
            {
                drone.playerDrCo.DetachRotateDrones(drone);
                if (CheckMove(drone.targetOre.transform.position))
                {
                    Mine();
                    doMove = false;
                }
                else
                {
                    moveTarget = drone.targetOre.transform.position;
                    doMove = true;
                }
            }
            else if (drone.targetOre == null)
            {
                moveTarget = returnPos;
                doMove = true;
            }

            if (moveTarget == returnPos && CheckMove(returnPos))
            {
                drone.playerDrCo.AttachRotateDrones(drone);
                doMove = false;
            }

            if (Vector2.Distance(myTransform.position, playerTransform.position) > maxDistance)
            {
                drone.targetOre = null;
                moveTarget = returnPos;
                doMove = true;
            }
        }

        public override void FixedRun()
        {
            if (doMove)
            {
                myTransform.position = Vector2.MoveTowards(myTransform.position, moveTarget, moveSpeed * Time.fixedDeltaTime);
            }
        }

        bool CheckMove(Vector3 target)
        {
            if (Vector2.Distance(myTransform.position, target) <= 0.2f)
                return true;
            else
            {
                return false;
            }
        }

        void Mine()
        {
            if (mineTime <= 0)
            {
                if (drone.targetOre.canGiveOre)
                {
                    playerInventory.AddItem(drone.targetOre.item, itemPerTime);
                    drone.targetOre.Give(itemPerTime);
                }
                mineTime = mineSpeed;
            }
            else
            {
                mineTime -= Time.deltaTime;
            }
        }
    }
}
