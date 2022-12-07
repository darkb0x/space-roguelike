using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret
{
    using Enemy;
    using Bullets;

    public class TurretLaser : TurretAI
    {
        [Header("TurretLaser")]
        [SerializeField] private float laserDistance;
        [SerializeField] private LayerMask layers;

        public void OnDrawGizmosSelected()
        {
            if (shotPos == null | turret_canon == null)
                return;

            Gizmos.color = Color.red;
            Debug.DrawRay(shotPos.position, turret_canon.right * laserDistance, Color.blue);
        }

        public override void Attack()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(shotPos.position, turret_canon.right, laserDistance, layers);
            Vector3 hitPos = (shotPos.position + turret_canon.right * laserDistance);

            foreach (var hit in hits)
            {
                hitPos = hit.point;

                if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(damage);
                }
                else
                {
                    break;
                }
            }
            BulletLine b = Instantiate(bulletPrefab, shotPos.position, Quaternion.identity).GetComponent<BulletLine>();
            b.Init(hitPos);
        }
    }
}
