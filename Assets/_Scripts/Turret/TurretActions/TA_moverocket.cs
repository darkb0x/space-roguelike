using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "turret action moverocket", menuName = "Turret/AI/new moverocket")]
public class TA_moverocket : TurretAction
{
    [SerializeField] private float startReloadTime;
    [SerializeField] private int maxRocketAmountInMagazine = 6;

    float reloadTime;
    float attackTime;
    int rockets;

    public override void Init()
    {
        rockets = maxRocketAmountInMagazine;
        reloadTime = startReloadTime;
        attackTime = turret.timeBtwAttack;
    }

    public override void Run()
    {
        if(turret.currentEnemy != null)
        {
            Attack();
        }

        if(rockets <= 0)
        {
            Reload();
        }
    }

    void Attack()
    {
        if (rockets > 0)
        {
            if (attackTime <= 0)
            {
                Rocket r = Instantiate(turret.stats.bulletPrefab, turret.shotPos.position, turret.shotPos.rotation).GetComponent<Rocket>();
                r.Init(turret.stats.damage, turret.currentEnemy.transform);

                rockets--;

                attackTime = turret.timeBtwAttack;
            }
            else
            {
                attackTime -= Time.deltaTime;
            }
        }
    }

    void Reload()
    {
        if(reloadTime <= 0)
        {
            rockets = maxRocketAmountInMagazine;
            reloadTime = startReloadTime;
        }
        else
        {
            reloadTime -= Time.deltaTime;
        }
    }
}
