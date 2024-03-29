using UnityEngine;

namespace Game.Turret
{
    using Bullets;

    public class TurretMoverockets : Turret
    {
        [Header("TurretMoverockets")]
        [SerializeField] private int maxRocketsCount;
        [SerializeField] private float startReloadTime;

        private int currentRocketsCount;
        private float reloadTime;
        private bool isReloading = false;

        protected override void Awake()
        {
            base.Awake();

            currentRocketsCount = maxRocketsCount;
            reloadTime = startReloadTime;
        }

        protected override void Update()
        {
            if(currentRocketsCount <= 0)
            {
                Reload();
            }

            base.Update();
        }

        protected override void Attack()
        {
            if (isReloading)
                return;

            Rocket rocket = Instantiate(BulletPrefab, ShotPos.position, ShotPos.rotation).GetComponent<Rocket>();
            rocket.Init(Damage, currentEnemy.transform);
            currentRocketsCount--;

            int index = Random.Range(0, targets.Count - 1);
            currentEnemy = targets[index].GetComponent<Enemy.EnemyAI>();
        }

        private void Reload()
        {
            if (reloadTime <= 0)
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
