using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    using Enemy;

    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner instance;
        public void Awake() => instance = this;

        [SerializeField, ReadOnly] private List<EnemyTarget> AllTargets;

        public void AddTarget(EnemyTarget enemyTarget)
        {
            if(!AllTargets.Contains(enemyTarget))
            {
                AllTargets.Add(enemyTarget);
            }
        }
        public void RemoveTarget(EnemyTarget enemyTarget)
        {
            if (AllTargets.Contains(enemyTarget))
            {
                AllTargets.Remove(enemyTarget);
            }
        }
        public List<EnemyTarget> GetTargetList()
        {
            return AllTargets;
        }
    }
}
