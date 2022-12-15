using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret
{
    using Enemy;

    public class TurretFiregun : TurretAI
    {
        [Header("TurretFiregun")]
        [SerializeField] private float distance;
        [SerializeField] private float range = 1f;
        [SerializeField] private LayerMask layers;
        [Space]
        [SerializeField] private ParticleSystem fireParticles;
        [SerializeField] private float emissionValue = 70;
        [SerializeField] private float fireEnabledSpeed = 2;

        private void OnDrawGizmosSelected()
        {
            if (shotPos == null | turret_canon == null)
                return;

            Debug.DrawRay(shotPos.position, turret_canon.right * distance, Color.blue);
            Debug.DrawRay(shotPos.position, (turret_canon.right + turret_canon.up * range) * distance, Color.blue);
            Debug.DrawRay(shotPos.position, (turret_canon.right + turret_canon.up * -range) * distance, Color.blue);
        }

        public override void Start()
        {
            base.Start();

            fireParticles.gameObject.SetActive(false);
        }

        public override void Run()
        {
            var particleEmmision = fireParticles.emission;

            particleEmmision.rateOverTime = Mathf.Lerp(particleEmmision.rateOverTime.constantMax, enemyInZone ? emissionValue : 0, fireEnabledSpeed * Time.deltaTime);

            base.Run();
        }

        public override void Attack()
        {
            RaycastHit2D[] hits_left = Physics2D.RaycastAll(shotPos.position, turret_canon.right, distance, layers);
            RaycastHit2D[] hits_middle = Physics2D.RaycastAll(shotPos.position, turret_canon.right + turret_canon.up * range, distance, layers);
            RaycastHit2D[] hits_right = Physics2D.RaycastAll(shotPos.position, turret_canon.right + turret_canon.up * -range, distance, layers);
            foreach (var hit in hits_left)
            {
                if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(damage);
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
                    enemy.TakeDamage(damage);
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
                    enemy.TakeDamage(damage);
                }
                else
                {
                    break;
                }
            }
        }

        public override void Put()
        {
            base.Put();

            fireParticles.gameObject.SetActive(true);
        }
    }
}
