using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "drone protect mine", menuName = "Drone/new action protect")]
public class DA_protect : DroneAction
{
    float radiusValue = 0;
    Transform playerTransform;
    Transform myTransform;
    Vector3 moveTarget;
    bool doMove = false;
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
    }

    public override void Run()
    {
        // move
        Move();

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

    void Move()
    {
        if (doMove) radiusValue += Time.deltaTime * rotateSpeed;
        else radiusValue -= Time.deltaTime * rotateSpeed;

        if (radiusValue > 100) doMove = false;
        else if (radiusValue < 0) doMove = true;

        float x = Mathf.Cos(radiusValue) * radius;
        float y = Mathf.Sin(radiusValue) * radius;
        drone.spriteTransform.localPosition = new Vector3(x, y, 0);
        myTransform.position = Vector2.Lerp(myTransform.position, playerTransform.position, moveSpeed * Time.deltaTime);
    }
}
