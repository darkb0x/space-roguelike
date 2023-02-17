using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Drone
{
    using Bullets;

    [CreateAssetMenu(fileName = "drone action protect", menuName = "Game/Drone/new Action protect")]
    public class DA_protect : DroneAction
    {
        float radiusValue = 0;
        Transform playerTransform;
        Transform myTransform;
        float attackTime;

        [SerializeField] private float rotateSpeed;
        [SerializeField] private float moveSpeed = 0.5f;
        [SerializeField] private float radius;
        [SerializeField] private int damage = 1;
        [SerializeField] private float timeBtwAttack;
        [SerializeField] private float maxDistanceBtwPlayerAndEnemy = 4.3f;
        [Space]
        [SerializeField] private BulletTrail bullet;

        public override void Init()
        {
            playerTransform = player.transform;
            myTransform = drone.transform;
            attackTime = timeBtwAttack;

            drone.playerDrCo.AttachProtectorDrone(drone);
            drone.playerDrCo.AttachRotateDrones(drone);
        }

        public override void Run()
        {
            //attack
            if (drone.targetEnemy != null)
            {
                if (Vector2.Distance(playerTransform.position, drone.targetEnemy.transform.position) > maxDistanceBtwPlayerAndEnemy)
                {
                    drone.targetEnemy = null;
                }
                else
                {
                    Attack();
                }
            }
        }

        void Attack()
        {
            if (attackTime <= 0)
            {
                Instantiate(bullet.gameObject, drone.spriteTransform.position, Quaternion.identity).GetComponent<BulletTrail>().Init(drone.targetEnemy.transform.position);
                drone.targetEnemy.TakeDamage(damage);
                attackTime = timeBtwAttack;
            }
            else
            {
                attackTime -= Time.deltaTime;
            }
        }
    }
}
