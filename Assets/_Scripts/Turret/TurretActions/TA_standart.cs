using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "turret action standart", menuName = "Turret/new standart")]
public class TA_standart : TurretAction
{
    [Space]
    [SerializeField] private float time_smooth = 1.2f;

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

    void RotateToTarget(GameObject target)
    {
        if (turret.currentEnemy != null)
        {
            Vector3 dir = target.transform.position - turret.turret_canon.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion a = turret.turret_canon.rotation;
            Quaternion b = Quaternion.Euler(0, 0, angle);
            turret.turret_canon.rotation = Quaternion.Lerp(a, b, time_smooth); //сдела сглаживание поворота ибо так красивее
        }
        else
        {
            return;
        }
    }

    void Attack()
    {
        if (attackTime <= 0)
        {
            Bullet b = Instantiate(turret.bulletPrefab, turret.shotPos.position, turret.shotPos.rotation).GetComponent<Bullet>();
            b.Init(turret.damage);
            attackTime = turret.timeBtwAttack;
        }
        else
        {
            attackTime -= Time.deltaTime;
        }
    }

    public override void TriggerEnter(Collider2D col, TurretAI _turret)
    {
        if (turret == null) turret = _turret;

        if(col.tag == turret.enemyTag)
        {
            turret.currentEnemy = col.gameObject;
        }
    }
    public override void TriggerStay(Collider2D col, TurretAI _turret)
    {
        if (turret == null) turret = _turret;

        if (col.tag == turret.enemyTag)
        {
            turret.enemyInZone = true;
            if(turret.currentEnemy.GetComponent<EnemyAI>().hp <= 0)
            {
                turret.currentEnemy = col.gameObject;
            }
            if(turret.currentEnemy != null)
            {
                RotateToTarget(turret.currentEnemy);
            }
            else
            {
                turret.currentEnemy = col.gameObject;
            }
        }
    }
    public override void TriggerExit(Collider2D col, TurretAI _turret)
    {
        if (turret == null) turret = _turret;

        if(col.tag == turret.enemyTag)
        {
            turret.enemyInZone = false;
        }
    }
}
