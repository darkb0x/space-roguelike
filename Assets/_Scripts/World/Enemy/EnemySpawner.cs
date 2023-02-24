using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace Game
{
    using Enemy;

    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner instance;
        public void Awake() => instance = this;

        private List<EnemyAI> AllEnemies = new List<EnemyAI>();

        [SerializeField, ReadOnly] private List<EnemyTarget> AllTargets;
        [Space]
        [SerializeField] private EnemyData[] EnemyList;

        [Header("Spawn parameters")]
        [SerializeField] private float TimeBtwSpawnEnemy = 0.2f;
        [SerializeField] private float SpawnRadius = 1.2f;
        [SerializeField] private Transform[] SpawnPoints;

        [Header("Difficult")]
        [SerializeField] private float DifficultFactor = 1f;
        [SerializeField] private float EnemyAmountFactor = 1f;
        [SerializeField] private int SpawnScore = 70;
        [SerializeField] private int EnemyMaxAmount = 40;

        private void OnDrawGizmos()
        {
            foreach (var spawnPoint in SpawnPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(spawnPoint.position, SpawnRadius);
            }
        }

        private void Start()
        {
            SpawnScore = Mathf.RoundToInt(SpawnScore * EnemyAmountFactor);
        }

        private void Update()
        {
            if(Keyboard.current.gKey.isPressed)
            {
                SpawnScore = 70;

                StartSpawning();
            }
        }

        [Button]
        public void StartSpawning()
        {
            StartCoroutine(SpawnEnemies());
        }
        IEnumerator SpawnEnemies()
        {
            while(SpawnScore > 0)
            {
                if (AllEnemies.Count >= EnemyMaxAmount)
                {
                    break;
                }

                EnemyData enemyData = EnemyList[Random.Range(0, EnemyList.Length)];
                Transform spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
                float directionAccuracy = SpawnRadius;
                Vector2 direction = new Vector2(Random.Range(-directionAccuracy, directionAccuracy), Random.Range(-directionAccuracy, directionAccuracy));

                EnemyAI enemy = Instantiate(enemyData.EnemyPrefab, (Vector2)spawnPoint.position + direction, Quaternion.identity).GetComponent<EnemyAI>();
                enemy.Initialize(enemyData, DifficultFactor);

                SpawnScore -= enemyData.Cost;
                AllEnemies.Add(enemy);
                yield return new WaitForSeconds(TimeBtwSpawnEnemy);
            }
        }

        public void RemoveEnemy(EnemyAI enemyAI)
        {
            AllEnemies.Remove(enemyAI);

            if (AllEnemies.Count < EnemyMaxAmount && SpawnScore > 0)
            {
                StartSpawning();
            }
        }

        #region AllTargets list interact
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
        #endregion
    }
}
