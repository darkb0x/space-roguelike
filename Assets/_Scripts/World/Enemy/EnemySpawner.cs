using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace Game.Enemy
{
    using SaveData;

    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance;
        public void Awake() => Instance = this;

        [HideInInspector] public List<EnemyAI> AllEnemies = new List<EnemyAI>();

        [SerializeField, ReadOnly] public List<EnemyTarget> AllTargets;
        [Space]
        [SerializeField] private EnemyData[] EnemyList;

        [Header("Spawn parameters")]
        [SerializeField] private float TimeBtwSpawnEnemy = 0.2f;
        [SerializeField] private float SpawnRadius = 1.2f;
        [SerializeField] private Transform[] SpawnPoints;

        [Header("Difficult")]
        public float DifficultFactor = 1f;
        //public float EnemyAmountFactor = 1f;
        [SerializeField, ReadOnly] private float SpawnScore;
        [SerializeField] private int MaxSpawnScore = 70;
        public int EnemyMaxAmount = 40;

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
            SpawnScore = Mathf.RoundToInt(MaxSpawnScore * DifficultFactor);
        }

        #if UNITY_EDITOR
        private void Update()
        {
            if(Keyboard.current.gKey.isPressed)
            {
                SpawnScore = MaxSpawnScore;

                StartSpawning();
            }
        }
        #endif

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void StartSpawning()
        {
            StartCoroutine(SpawnEnemies(SpawnScore));
        }

        public void StartSpawning(int percentFromSpawnScore)
        {
            if (Application.isPlaying)
            {
                float score = SpawnScore / 100 * percentFromSpawnScore;
                StartCoroutine(SpawnEnemies(score));
            }
        }

        int currentSpawnScore = 0;
        IEnumerator SpawnEnemies(float spawnScore)
        {
            currentSpawnScore = Mathf.RoundToInt(spawnScore);
            while(currentSpawnScore > 0)
            {
                if (AllEnemies.Count >= EnemyMaxAmount)
                {
                    break;
                }

                List<EnemyData> enemyDatas = new List<EnemyData>();
                foreach (var data in EnemyList)
                {
                    if(currentSpawnScore >= data.Cost)
                    {
                        enemyDatas.Add(data);
                    }
                }
                if(enemyDatas.Count == 0)
                {
                    break;
                }

                EnemyData enemyData = enemyDatas[Random.Range(0, enemyDatas.Count)];
                Transform spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
                float directionAccuracy = SpawnRadius;
                Vector2 direction = new Vector2(Random.Range(-directionAccuracy, directionAccuracy), Random.Range(-directionAccuracy, directionAccuracy));

                EnemyAI enemy = Instantiate(enemyData.EnemyPrefab, (Vector2)spawnPoint.position + direction, Quaternion.identity).GetComponent<EnemyAI>();
                enemy.Initialize(enemyData, DifficultFactor);

                currentSpawnScore -= enemyData.Cost;
                AllEnemies.Add(enemy);

                yield return new WaitForSeconds(TimeBtwSpawnEnemy);
            }
        }
        public void RemoveEnemy(EnemyAI enemyAI)
        {
            if(AllEnemies.Contains(enemyAI))
            {
                AllEnemies.Remove(enemyAI);
            }

            if (AllEnemies.Count < EnemyMaxAmount && MaxSpawnScore > 0)
            {
                if(currentSpawnScore > 0)
                {
                    StartCoroutine(SpawnEnemies(currentSpawnScore));
                }
            }
        }

        public void ClearEnemies()
        {
            currentSpawnScore = 0;

            EnemyAI[] enemiesToClear = AllEnemies.ToArray();

            foreach (var enemy in enemiesToClear)
            {
                enemy.Die(true);
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
        public EnemyTarget[] GetTargetList()
        {
            return AllTargets.ToArray();
        }
        #endregion
    }
}
