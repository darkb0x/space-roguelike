using UnityEngine;

namespace Game.Turret
{
    using Enemy;
    using Bullets;

    public class TurretLaser : Turret
    {
        [Header("TurretLaser")]
        [SerializeField] private LayerMask Layers;
        [SerializeField] private float LaserDistance;

        private void OnDrawGizmosSelected()
        {
            if(TurretCanon != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(ShotPos.position, ShotPos.position + (TurretCanon.right * LaserDistance));
            }
        }

        protected override void Attack()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(ShotPos.position, TurretCanon.right, LaserDistance, Layers);
            Vector3 hitPos = (ShotPos.position + TurretCanon.right * LaserDistance);

            foreach (var hit in hits)
            {
                hitPos = hit.point;

                if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(Damage);
                }
                else
                {
                    break;
                }
            }

            BulletLine b = Instantiate(BulletPrefab, ShotPos.position, Quaternion.identity).GetComponent<BulletLine>();
            b.Init(hitPos);
        }
    }
}
