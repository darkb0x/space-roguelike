using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "turret action laser", menuName = "Turret/AI/new laser")]
public class TA_laser : TurretAction
{
    [SerializeField] private float laserDistance;
    [SerializeField] private LayerMask layers;
    float attackTime;

    public override void Init()
    {
        attackTime = turret.timeBtwAttack;
    }

    public override void Run()
    {
        Debug.DrawRay(turret.shotPos.position, turret.turret_canon.right, Color.red);
        if (turret.enemyInZone)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (attackTime <= 0)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(turret.shotPos.position, turret.turret_canon.right, laserDistance, layers);
            Vector3 hitPos = Vector3.zero;
            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(turret.damage);
                }
                else
                {
                    hitPos = hit.point;
                    break;
                }
            }
            BulletTrail b = Instantiate(turret.bulletPrefab, turret.shotPos.position, Quaternion.identity).GetComponent<BulletTrail>();
            b.Init(hitPos);

            attackTime = turret.timeBtwAttack;
        }
        else
        {
            attackTime -= Time.deltaTime;
        }
    }
}
