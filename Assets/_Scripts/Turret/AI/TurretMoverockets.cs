using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret.AI
{
    using Bullets;

    [CreateAssetMenu(fileName = "Turret rockets AI", menuName = "Turret/AI/new rockets AI")]
    public class TurretMoverockets : TurretAI
    {
        public const string ROCKETS_COUNT = "RocketsCount";
        public const string RELOAD_TIME = "ReloadTime";

        // rockets count
        private int maxRocketsCount;
        private int currentRocketsCount;
        // reload time
        private float startReloadTime;
        private float reloadTime;

        private bool isReloading = false;

        public override void Initialize()
        {
            base.Initialize();

            maxRocketsCount = (int)data.GetVariable(ROCKETS_COUNT);
            startReloadTime = (float)data.GetVariable(RELOAD_TIME);

            currentRocketsCount = maxRocketsCount;
            reloadTime = startReloadTime;
        }

        public override void Run()
        {
            if (currentRocketsCount <= 0)
            {
                Reload();
            }

            if (!turret.isPicked)
                base.Run();
        }

        public override void Attack()
        {
            if (isReloading)
                return;

            Rocket rocket = Instantiate(data._bulletPrefab, turret.shotPos.position, turret.shotPos.rotation).GetComponent<Rocket>();
            rocket.Init(data._damage, turret.currentEnemy);
            currentRocketsCount--;
        }

        private void Reload()
        {
            if(reloadTime <= 0)
            {
                currentRocketsCount = maxRocketsCount;
                reloadTime = startReloadTime;
                isReloading = false;
            }
            else
            {
                reloadTime -= Time.deltaTime;
                isReloading = true;
            }
        }
    }
}
