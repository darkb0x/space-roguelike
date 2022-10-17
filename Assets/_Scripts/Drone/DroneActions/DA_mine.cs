using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float minDistance = 1.2f; // Between drone and player if target ore is null;
    [SerializeField] private float moveSpeed = 0.2f;
    [SerializeField] private float mineSpeed = 1.3f;
    [SerializeField] private int itemPerTime = 1;

    public override void Init()
    {
        playerTransform = player.transform;
        myTransform = drone.transform;
        playerInventory = FindObjectOfType<PlayerInventory>();

        drone.playerDrCo.AttachMinerDrone(drone);
        mineTime = mineSpeed;
    }

    public override void Run()
    {
        if (drone.targetOre != null && Vector2.Distance(myTransform.position, playerTransform.position) < maxDistance)
        {
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
        else if (drone.targetOre == null && Vector2.Distance(myTransform.position, playerTransform.position) > minDistance)
        {
            moveTarget = playerTransform.position;
            doMove = true;
        }

        if (Vector2.Distance(myTransform.position, playerTransform.position) > maxDistance)
        {
            drone.targetOre = null;
        }
    }

    public override void FixedRun()
    {
        if(doMove)
        {
            myTransform.position = Vector2.MoveTowards(myTransform.position, moveTarget, moveSpeed * Time.fixedDeltaTime);
        }
    }

    bool CheckMove(Vector3 target)
    {
        if (Vector2.Distance(myTransform.position, target) <= 0.3f)
            return true;
        else
        {
            return false;
        }
    }

    void Mine()
    {
        if(mineTime <= 0)
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
