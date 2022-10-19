using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "turret action firegun", menuName = "Turret/AI/new firegun")]
public class TA_firegun : TurretAction
{
    [SerializeField] private float laserDistance;
    [SerializeField] private LayerMask layers;
    [Space]
    [SerializeField] private float emissionValue = 70;
    [SerializeField] private float fireEnabledSpeed = 2;

    float attackTime;

    public override void Init()
    {
        attackTime = turret.timeBtwAttack;
    }

    public override void Run()
    {
        Debug.DrawLine(turret.shotPos.position, turret.shotPos.position + turret.turret_canon.right * laserDistance);
        turret.fireParticles.emissionRate = Mathf.Lerp(turret.fireParticles.emissionRate, turret.enemyInZone ? emissionValue : 0, fireEnabledSpeed * Time.deltaTime);
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
            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(turret.damage);
                }
                else
                {
                    break;
                }
            }
            attackTime = turret.timeBtwAttack;
        }
        else
        {
            attackTime -= Time.deltaTime;
        }
    }
}
