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
    public interface IMovableTarget
    {
        public Vector3 GetMoveDirection();
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
        [SerializeField] private bool AddObjectToListAtStart = true;

        private IDamagable damagableObject;
        private IMovableTarget movableObject;

        public void Initialize(IDamagable damagableObj, IMovableTarget movableObj)
        {
            damagableObject = damagableObj;
            movableObject = movableObj;

            if (AddObjectToListAtStart)
                Singleton.Get<EnemySpawner>().AddTarget(this);
        }

        public void Hurt(float dmg)
        {
            if (damagableObject != null)
                damagableObject.Damage(dmg, this);
            else
                Debug.LogWarning($"{transform.parent.name} > {gameObject.name}/EnemyTarget.cs/damagableObject is null (Maybe you didn't initialized it.)");
        }
        public Vector3 GetMoveDirection()
        {
            if (movableObject == null)
                return Vector3.zero;
            else
                return movableObject.GetMoveDirection();
        }
    }
}
