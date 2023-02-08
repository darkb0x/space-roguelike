using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IDamagable
    {
        public void Damage(float dmg, Enemy.EnemyTarget enemyTarget)
        {
            if(enemyTarget.IsDamaging)
            {
                enemyTarget.Health -= dmg;

                if (enemyTarget.Health <= 0)
                {
                    Die();
                }
            }
        }
        public void Die();
    }
}

namespace Game.Enemy
{
    public enum EnemyTargetPriority
    {
        Player = 0,
        Turret = 1,
        Drill = 2
    }

    public class EnemyTarget : MonoBehaviour
    {
        public EnemyTargetPriority Priority;
        [Space]
        public bool IsDamaging = true;
        [NaughtyAttributes.EnableIf("IsDamaging")] public float Health;

        private IDamagable damagableObject;

        public void Initialize(IDamagable obj)
        {
            damagableObject = obj;
            EnemySpawner.instance.AddTarget(this);
        }

        public void Hurt(float dmg)
        {
            if (damagableObject != null)
                damagableObject.Damage(dmg, this);
            else
                Debug.LogWarning($"{transform.parent.name} > {gameObject.name}/EnemyTarget.cs/damagableObject is null (Maybe you didn't initialized it.)");
        }
    }
}
