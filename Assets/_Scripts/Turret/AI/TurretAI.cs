using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret.AI
{
    using Bullets;

    public abstract class TurretAI : ScriptableObject
    {
        [HideInInspector] public Turret turret;
        [HideInInspector] public TurretData data;

        public virtual void Initialize() { }

        public virtual void Run ()
        {
            if (!turret.enemyInZone)
                return;

            if (turret.currentTimeBtwAttacks <= 0)
            {
                Attack();
                turret.currentTimeBtwAttacks = data._timeBtwAttack;
            }
            else
            {
                turret.currentTimeBtwAttacks -= Time.deltaTime;
            }
        }

        public virtual void Attack()
        {
            float recoil = data._recoil;
            GameObject bulletPrefab = data._bulletPrefab;
            float damage = data._damage;

            float recoilRotation = Random.Range(-recoil, recoil);
            Bullet bullet = Instantiate(bulletPrefab, turret.ShotPos.position, turret.ShotPos.rotation).GetComponent<Bullet>();
            bullet.gameObject.transform.Rotate(0, 0, recoilRotation);
            bullet.Init(damage);
        }
    }
}
