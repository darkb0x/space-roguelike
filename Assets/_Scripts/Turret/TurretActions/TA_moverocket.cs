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
    Rocket currentRocket;

    public override void Init()
    {
        rockets = maxRocketAmountInMagazine;
        reloadTime = startReloadTime;
        attackTime = turret.timeBtwAttack;
    }

    public override void Run()
    {
        if (turret.currentEnemy != null)
        {
            Attack();
        }
        else
        {
            if (currentRocket != null)
            {
                currentRocket.StopControl();
                currentRocket = null;
            }
        }

        if (rockets <= 0)
        {
            Reload();
        }
    }

    void Attack()
    {
        if(currentRocket != null)
        {
            currentRocket.ControlMoveToTarget(turret.currentEnemy.transform.position);
        }
        else
        {
            if(rockets > 0)
            {
                if (attackTime <= 0)
                {
                    SpawnRocket();
                    attackTime = turret.timeBtwAttack;
                }
                else
                {
                    attackTime -= Time.deltaTime;
                }
            }
        }
    }

    void SpawnRocket()
    {
        Rocket r = Instantiate(turret.stats.bulletPrefab, turret.shotPos.position, turret.shotPos.rotation).GetComponent<Rocket>();
        r.Init(turret.stats.damage);

        currentRocket = r;

        rockets--;
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
