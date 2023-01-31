using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret.AI
{
    using Enemy;

    [CreateAssetMenu(fileName = "Turret firegun AI", menuName = "Turret/AI/new firegun AI")]
    public class TurretFiregun : TurretAI
    {
        public const string DISTANCE = "Distance";
        public const string RANGE = "Range";

        [SerializeField] private LayerMask Layers;
        [SerializeField] private float FireEnabledSpeed = 2;

        private float distance;
        private float range;
        private float startEmissionValue = 30f;
        private ParticleSystem fireParticle;

        public override void Initialize()
        {
            base.Initialize();

            distance = (float)data.GetVariable(DISTANCE);
            range = (float)data.GetVariable(RANGE);

            fireParticle = Instantiate(data._bulletPrefab, turret.shotPos).GetComponent<ParticleSystem>();
            fireParticle.gameObject.SetActive(false);
        }

        public override void Run()
        {
            if(!turret.isPicked)
                base.Run();

            float targetEmmision = 0f;
            if(turret.isPicked)
            {
                targetEmmision = 0f;
            }
            else
            {
                fireParticle.gameObject.SetActive(true);

                if (turret.enemyInZone)
                {
                    targetEmmision = startEmissionValue;
                }
            }

            ParticleSystem.EmissionModule emmision = fireParticle.emission;
            emmision.rateOverTime = Mathf.Lerp(emmision.rateOverTime.constantMax, targetEmmision, FireEnabledSpeed*Time.deltaTime);
        }

        public override void Attack()
        {
            Transform shotPos = turret.shotPos;
            Transform turretCanon = turret.TurretCanon;

            RaycastHit2D[] hits_left = Physics2D.RaycastAll(shotPos.position, turretCanon.right, distance, Layers);
            RaycastHit2D[] hits_middle = Physics2D.RaycastAll(shotPos.position, turretCanon.right + turretCanon.up * range, distance, Layers);
            RaycastHit2D[] hits_right = Physics2D.RaycastAll(shotPos.position, turretCanon.right + turretCanon.up * -range, distance, Layers);

            float damage = data._damage;

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
    }
}
