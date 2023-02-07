using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void Damage(float dmg, Game.Enemy.EnemyTarget enemyTarget)
    {
        if (enemyTarget.Health <= 0)
        {
            Die();
        }
    }
    protected abstract void Die();
}

namespace Game.Enemy
{
    public enum EnemyTargetPriority
    {
        None = 0
    }

    public class EnemyTarget : MonoBehaviour
    {
        public EnemyTargetPriority Priority;
        [Space]
        public float Health;

        private IDamagable damagableObject;

        public void Initialize(IDamagable obj)
        {
            damagableObject = obj;
        }

        public void Hurt(float dmg)
        {
            Health -= dmg;

            damagableObject.Damage(dmg, this);
        }
    }
}
