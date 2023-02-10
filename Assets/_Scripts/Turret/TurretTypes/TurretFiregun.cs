using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret
{
    using Enemy;

    public class TurretFiregun : Turret
    {
        [Header("TurretFiregun")]
        [SerializeField] private LayerMask Layers;
        [SerializeField] private float Distance;
        [SerializeField] private float Range;
        [Space]
        [SerializeField] private float startEmissionValue = 30f;
        [SerializeField] private float FireEnabledSpeed = 2;
        [SerializeField] private ParticleSystem fireParticle;

        private void OnDrawGizmosSelected()
        {
            if(TurretCanon != null && ShotPos != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(ShotPos.position, ShotPos.position + (TurretCanon.right * Distance));
                Gizmos.DrawLine(ShotPos.position, ShotPos.position + (TurretCanon.right + TurretCanon.up * Range) * Distance);
                Gizmos.DrawLine(ShotPos.position, ShotPos.position + (TurretCanon.right + -TurretCanon.up * Range) * Distance);
            }
        }

        protected override void Start()
        {
            base.Start();

            fireParticle.gameObject.SetActive(false);
        }

        protected override void Update()
        {
            float targetEmmision = 0f;
            if (isPicked)
            {
                targetEmmision = 0f;
            }
            else
            {
                fireParticle.gameObject.SetActive(true);

                if (enemyInZone)
                {
                    targetEmmision = startEmissionValue;
                }
            }

            ParticleSystem.EmissionModule emmision = fireParticle.emission;
            emmision.rateOverTime = Mathf.Lerp(emmision.rateOverTime.constantMax, targetEmmision, FireEnabledSpeed * Time.deltaTime);

            base.Update();
        }

        protected override void Attack()
        {
            RaycastHit2D[] hits_left = Physics2D.RaycastAll(ShotPos.position, TurretCanon.right, Distance, Layers);
            RaycastHit2D[] hits_middle = Physics2D.RaycastAll(ShotPos.position, TurretCanon.right + TurretCanon.up * Range, Distance, Layers);
            RaycastHit2D[] hits_right = Physics2D.RaycastAll(ShotPos.position, TurretCanon.right + TurretCanon.up * -Range, Distance, Layers);

            foreach (var hit in hits_left)
            {
                if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(Damage);
                }
                else
                {
                    break;
                }
            }
            foreach (var hit in hits_middle)
            {
                if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(Damage);
                }
                else
                {
                    break;
                }
            }
            foreach (var hit in hits_right)
            {
                if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(Damage);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
