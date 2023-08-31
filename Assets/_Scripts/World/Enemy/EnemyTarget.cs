using UnityEngine;

namespace Game.Enemy
{
    public class EnemyTarget : MonoBehaviour
    {
        public EnemyTargetPriority Priority;
        [Space]
        public bool IsDamaging = true;
        [NaughtyAttributes.EnableIf("IsDamaging")] public float Health;
        [SerializeField] private bool AddObjectToListAtStart = true;

        private IDamagable damagableObject;
        private IMovableTarget movableObject;

        public void Initialize(IDamagable damagableObj, IMovableTarget movableObj = null)
        {
            damagableObject = damagableObj;
            movableObject = movableObj;

            if (AddObjectToListAtStart)
                ServiceLocator.GetService<EnemySpawner>().AddTarget(this);
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
