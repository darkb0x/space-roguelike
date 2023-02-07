using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void Damage(float dmg);
    public void Die();
}

namespace Game.Enemy
{
    public enum EnemyTargetPriority
    {
        Turret = 0, 
        Drill = 1,
        Player = 2
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

            damagableObject.Damage(dmg);

            if (Health <= 0)
                damagableObject.Die();
        }
    }
}
