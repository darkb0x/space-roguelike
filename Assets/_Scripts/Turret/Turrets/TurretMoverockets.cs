using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret
{
    using Bullets;

    public class TurretMoverockets : TurretAI
    {
        [Header("TurretMoverockets")]
        [SerializeField] private float startReloadTime;
        [SerializeField] private int maxRocketAmountInMagazine = 6;

        float reloadTime;
        int rockets;

        public override void Start()
        {
            base.Start();

            rockets = maxRocketAmountInMagazine;
            reloadTime = startReloadTime;
        }

        public override void Run()
        {
            base.Run();

            if(rockets <= 0)
            {
                Reload();
            }
        }

        public override void Attack()
        {
            if (rockets <= 0)
            {
                return;
            }  

            Rocket r = Instantiate(bulletPrefab, shotPos.position, shotPos.rotation).GetComponent<Rocket>();
            r.Init(damage, currentEnemy);

            rockets--;
        }

        private void Reload()
        {
            if (reloadTime <= 0)
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
}
