using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "turret action standart", menuName = "Turret/AI/new standart")]
public class TA_standart : TurretAction
{
    float attackTime;

    public override void Init()
    {
        attackTime = turret.timeBtwAttack;
    }

    public override void Run()
    {
        if(turret.enemyInZone)
        {
            Attack(); 
        }
    }

    void Attack()
    {
        if (attackTime <= 0)
        {
            float z = Random.Range(-turret.recoil, turret.recoil);
            Bullet b = Instantiate(turret.bulletPrefab, turret.shotPos.position, turret.shotPos.rotation).GetComponent<Bullet>();
            b.gameObject.transform.Rotate(0, 0, z);
            b.Init(turret.damage);
            attackTime = turret.timeBtwAttack;
        }
        else
        {
            attackTime -= Time.deltaTime;
        }
    }
}
