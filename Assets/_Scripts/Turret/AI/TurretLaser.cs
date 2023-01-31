using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret.AI
{
    using Enemy;
    using Bullets;

    [CreateAssetMenu(fileName = "Turret laser AI", menuName = "Turret/AI/new laser AI")]
    public class TurretLaser : TurretAI
    {
        public const string LASER_DISTANCE = "LaserDistance";

        [SerializeField] private LayerMask Layers;

        private float laserDistance;

        public override void Initialize()
        {
            base.Initialize();

            laserDistance = (float)data.GetVariable(LASER_DISTANCE);
        }

        public override void Run()
        {
            if(!turret.isPicked)
                base.Run();
        }

        public override void Attack()
        {
            Transform shotPos = turret.shotPos;
            Transform turretCanon = turret.TurretCanon;

            RaycastHit2D[] hits = Physics2D.RaycastAll(shotPos.position, turretCanon.right, laserDistance, Layers);
            Vector3 hitPos = (shotPos.position + turretCanon.right * laserDistance);

            foreach (var hit in hits)
            {
                hitPos = hit.point;

                if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(data._damage);
                }
                else
                {
                    break;
                }
            }

            BulletLine b = Instantiate(data._bulletPrefab, shotPos.position, Quaternion.identity).GetComponent<BulletLine>();
            b.Init(hitPos);
        }
    }
}
