using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "turret action firegun", menuName = "Turret/AI/new firegun")]
public class TA_firegun : TurretAction
{
    [SerializeField] private float distance;
    [SerializeField] private float range = 1f;
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
        Debug.DrawRay(turret.shotPos.position, turret.turret_canon.right * distance, Color.blue);
        Debug.DrawRay(turret.shotPos.position, (turret.turret_canon.right + turret.turret_canon.up * range) * distance, Color.blue);
        Debug.DrawRay(turret.shotPos.position, (turret.turret_canon.right + turret.turret_canon.up * -range) * distance, Color.blue);

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
            RaycastHit2D[] hits_left = Physics2D.RaycastAll(turret.shotPos.position, turret.turret_canon.right, distance, layers);
            RaycastHit2D[] hits_middle = Physics2D.RaycastAll(turret.shotPos.position, turret.turret_canon.right + turret.turret_canon.up * range, distance, layers);
            RaycastHit2D[] hits_right = Physics2D.RaycastAll(turret.shotPos.position, turret.turret_canon.right + turret.turret_canon.up * -range, distance, layers);
            foreach (var hit in hits_left)
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
            foreach (var hit in hits_middle)
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
            foreach (var hit in hits_right)
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
