using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Enemy
{
    public class EnemySpawner : MonoBehaviour, ISingleton
    {
        [HideInInspector] public List<EnemyAI> AllEnemies = new List<EnemyAI>();

        [SerializeField, ReadOnly] private List<EnemyTarget> AllTargets;
        [Space]
        [SerializeField] private EnemyData[] EnemyList;

        [Header("Spawn parameters")]
        [SerializeField] private float TimeBtwSpawnEnemy = 0.2f;
        [SerializeField] private float TimeBtwSpawnEnemyInWave = 1f;
        [SerializeField] private float SpawnRadius = 1.2f;
        [SerializeField] private Transform[] SpawnPoints;

        [Header("Difficult")]
        public float DifficultFactor = 1f;
        [SerializeField, ReadOnly] private float SpawnScore;
        [SerializeField] private int MaxSpawnScore = 70;
        public int EnemyMaxAmount = 40;

        private bool inWave = false;

        public System.Action<EnemyTarget> OnTargetAdded;
        public System.Action<EnemyTarget> OnTargetRemoved;

        private SessionManager SessionManager;

        private void OnDrawGizmos()
        {
            foreach (var spawnPoint in SpawnPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(spawnPoint.position, SpawnRadius);
            }
        }

        private void Awake()
        {
            Singleton.Add(this);
        }
        private void Start()
        {
            SessionManager = Singleton.Get<SessionManager>();

            SpawnScore = Mathf.RoundToInt(MaxSpawnScore * DifficultFactor);
        }

        public void StartWave(float start, float end)
        {
            if (!inWave)
            {
                StartCoroutine(SpawnEnemiesWave(start, end));

                inWave = true;
            }
        }

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

        private IEnumerator SpawnEnemiesWave(float startTime, float endTime)
        {
            LogUtility.WriteLog($"Start wave. Ends on: {endTime}");

            while (SessionManager.currentTime < startTime)
            {
                yield return null;
            }
            while(SessionManager.currentTime < endTime)
            {
                yield return new WaitForSeconds(TimeBtwSpawnEnemyInWave);

                if (AllEnemies.Count >= EnemyMaxAmount)
                {
                    continue;
                }

                EnemyData enemyData = EnemyList[Random.Range(0, EnemyList.Length)];
                Transform spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
                float directionAccuracy = SpawnRadius;
                Vector2 direction = new Vector2(Random.Range(-directionAccuracy, directionAccuracy), Random.Range(-directionAccuracy, directionAccuracy));

                EnemyAI enemy = Instantiate(enemyData.EnemyPrefab, (Vector2)spawnPoint.position + direction, Quaternion.identity).GetComponent<EnemyAI>();
                enemy.Initialize(enemyData, DifficultFactor);

                AllEnemies.Add(enemy);

                LogUtility.WriteLog($"Enemy spawned! Data: enemy_data='{enemyData.EnemyPrefab.name}'. Time: {SessionManager.currentTime}");
            }

            inWave = false;
        }

        int currentSpawnScore = 0;
        private IEnumerator SpawnEnemies(float spawnScore)
        {
            currentSpawnScore = Mathf.RoundToInt(spawnScore);
            LogUtility.WriteLog($"Start spawn enemy. Spawn score: {currentSpawnScore}");
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

                LogUtility.WriteLog($"Enemy spawned! Data: enemy_data='{enemyData.EnemyPrefab.name}', cost={enemyData.Cost}. Current score: {currentSpawnScore}");

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
                OnTargetAdded?.Invoke(enemyTarget);
            }
        }
        public void RemoveTarget(EnemyTarget enemyTarget)
        {
            if (AllTargets.Contains(enemyTarget))
            {
                AllTargets.Remove(enemyTarget);
                OnTargetRemoved?.Invoke(enemyTarget);
            }
        }
        public List<EnemyTarget> GetTargetList()
        {
            return AllTargets;
        }
        #endregion
    }
}
