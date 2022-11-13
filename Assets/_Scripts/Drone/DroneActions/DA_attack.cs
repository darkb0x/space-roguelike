using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Drone
{
    using Bullets;

    [CreateAssetMenu(fileName = "drone action attack", menuName = "Drone/new action attack")]
    public class DA_attack : DroneAction
    {
        float radiusValue = 0;
        Transform playerTransform;
        Transform myTransform;
        Vector3 moveTarget;
        bool doRotate = true;
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

            drone.playerDrCo.AttachAttackerDrone(drone);
        }

        public override void Run()
        {
            // rotate
            Rotate();

            // attack
            if (drone.targetEnemy != null)
            {
                if (Vector2.Distance(playerTransform.position, drone.targetEnemy.transform.position) > maxDistanceBtwPlayerAndEnemy)
                {
                    drone.targetEnemy = null;
                    moveTarget = playerTransform.position;
                }
                else
                {
                    moveTarget = drone.targetEnemy.transform.position;
                    Attack();
                }
            }
            else
            {
                moveTarget = playerTransform.position;
            }
        }

        public override void FixedRun()
        {
            myTransform.position = Vector2.MoveTowards(myTransform.position, moveTarget, moveSpeed * Time.fixedDeltaTime);
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

        void Rotate()
        {
            radiusValue += Time.deltaTime * drone.playerDrCo.droneRotationSpeed;

            float x = Mathf.Cos(radiusValue) * radius;
            float y = Mathf.Sin(radiusValue) * radius;
            drone.spriteTransform.localPosition = new Vector3(x, y, 0);
        }
    }
}
