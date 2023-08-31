using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

namespace Game.Turret
{
    using Player;
    using Player.Inventory;
    using Player.Pick;
    using Enemy;

    public abstract class Turret : MonoBehaviour, IDamagable, ICraftableBuild
    {
        private const float STANDART_ROTATION_TIME = 5f;

        [System.Serializable]
        public struct DroppedItem
        {
            public InventoryItem Item;
            public int Amount;
        }
        private bool isFacingRight = true;
        protected bool playerInZone { get; private set; }
        protected bool enemyInZone { get; private set; }

        private float currentTimeBtwAttacks;
        private Vector3 targetPlacePosition;

        private PlayerController player;

        [SerializeField] private bool InitializeOnStart = false;

        [Header("Turret variables")]
        [SerializeField] protected GameObject BulletPrefab;
        [SerializeField] protected float Damage = 1;
        [SerializeField] protected float TimeBtwAttack = 0.3f;
        [SerializeField] protected float Recoil = 0f;
        [Space]
        [SerializeField] private List<DroppedItem> DroppedItems = new List<DroppedItem>(1);

        [Header("Turret survivability")]
        [SerializeField] private EnemyTarget EnemyTarget;
        [field: SerializeField] public float Health { get; protected set; }
        [ReadOnly] public float currentHealth;
        [Space]
        [SerializeField] private GameObject DestroyParticle;

        [Header("Turret/Canon")]
        [SerializeField] private bool ChangeRotationTime ;
        [EnableIf("ChangeRotationTime"), SerializeField] protected float TurretRotateTime = STANDART_ROTATION_TIME;
        [EnableIf("ChangeRotationTime"), SerializeField] protected float TurretBackRotateTime = STANDART_ROTATION_TIME;
        [Space]
        [SerializeField] protected Transform TurretCanon;
        [SerializeField] protected Transform ShotPos;


        [Header("Enemy detecion")]
        [Tag, SerializeField] protected string EnemyTag = "Enemy";
        [SerializeField] protected bool UseEnemyTrajectory = false;
        [Space]
        protected EnemyAI currentEnemy;
        protected Vector2 enemyTrajectory;
        [ReadOnly] public List<GameObject> targets = new List<GameObject>();

        [Header("Pick object")]
        [SerializeField] protected float MendatoryFreeRadius = 0.5f;
        [SerializeField] protected LayerMask ConflictedLayers;
        public PickedObjectPreRenderrer PreRenderPlaceObject;
        [field: SerializeField] public bool isPicked { get; protected set; }
        
        [Header("Collisions")]
        [SerializeField] protected CircleCollider2D EnemyDetectionCollider;
        [SerializeField] protected CircleCollider2D PlayerDetectionCollider;

        [Header("Other")]
        [Tag, SerializeField] protected string PlayerTag = "Player";

        private EnemySpawner _enemySpawner;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, MendatoryFreeRadius);

            if (currentEnemy != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentEnemy.transform.position);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(currentEnemy.transform.position, currentEnemy.transform.position + (Vector3)currentEnemy.rb.velocity / 2);
            }
        }

        protected virtual void Awake()
        {
            _enemySpawner = ServiceLocator.GetService<EnemySpawner>();

            if(!ChangeRotationTime)
            {
                TurretRotateTime = STANDART_ROTATION_TIME;
                TurretBackRotateTime = STANDART_ROTATION_TIME;
            }

            EnemyTarget.Initialize(this);

            if (InitializeOnStart) 
            {
                currentTimeBtwAttacks = TimeBtwAttack;
                currentHealth = Health;

                PreRenderPlaceObject.gameObject.SetActive(false);
            }
        }

        public virtual void Initialize(PlayerController p) // ICraftableBuild func
        {
            player = p;

            currentTimeBtwAttacks = TimeBtwAttack;
            currentHealth = Health;

            isPicked = true;
            EnemyDetectionCollider.enabled = false;

            PreRenderPlaceObject.gameObject.SetActive(true);
            player.Build.Pick(this);
        }

        #region Updates
        protected virtual void Update()
        {
            if (isPicked)
            {
                Vector2 offset = new Vector2(0.5f, 0.5f);
                targetPlacePosition = new Vector2(Mathf.Round(player.transform.position.x), Mathf.Round(player.transform.position.y)) + offset;

                PreRenderPlaceObject.transform.position = targetPlacePosition;

                return;
            }

            #region rotation
            if (currentEnemy != null)
            {
                RotateToTarget(currentEnemy.transform);
            }
            else
            {
                GetRotateBack();
            }

            if (TurretCanon.eulerAngles.z != 0)
            {
                float rotZ = TurretCanon.eulerAngles.z;

                if (rotZ < 180)
                {
                    if (rotZ > 90 && isFacingRight)
                    {
                        FlipCanonTurret();
                    }
                    if (rotZ < 90 && !isFacingRight)
                    {
                        FlipCanonTurret();
                    }
                }
                else
                {
                    if (rotZ > 270 && !isFacingRight)
                    {
                        FlipCanonTurret();
                    }
                    if (rotZ < 270 && isFacingRight)
                    {
                        FlipCanonTurret();
                    }
                }
            }
            else
            {
                if (!isFacingRight)
                    FlipCanonTurret();
            }
            #endregion


            #region attacking
            enemyInZone = (targets.Count > 0);

            if (enemyInZone)
            {
                if (currentEnemy == null)
                    currentEnemy = GetNearestEnemy();

                if (currentTimeBtwAttacks <= 0)
                {
                    if (!isPicked)
                        Attack();

                    currentTimeBtwAttacks = TimeBtwAttack;
                    currentEnemy = GetNearestEnemy();
                }
                else
                {
                    currentTimeBtwAttacks -= Time.deltaTime;
                }
            }
            #endregion
        }
        #endregion

        #region Collision triggers
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == PlayerTag)
            {
                playerInZone = true;
            }
            if (collision.tag == EnemyTag)
            {
                if (!targets.Contains(collision.gameObject))
                    targets.Add(collision.gameObject);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == EnemyTag)
            {
                targets.Remove(collision.gameObject);
            }

            if (collision.tag == PlayerTag)
            {
                playerInZone = false;
            }
        }
        #endregion

        #region Utilties
        private void RotateToTarget(Transform target)
        {
            if (currentEnemy != null)
            {
                Vector3 dir;

                if (UseEnemyTrajectory)
                {
                    dir = (Vector3)enemyTrajectory - TurretCanon.position;
                }
                else
                {
                    dir = target.position - TurretCanon.position;
                }

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion a = TurretCanon.rotation;
                Quaternion b = Quaternion.Euler(0, TurretCanon.rotation.y, angle);
                TurretCanon.rotation = Quaternion.Lerp(a, b, TurretRotateTime * Time.deltaTime);
            }
            else
            {
                return;
            }
        }
        private void GetRotateBack()
        {
            Quaternion a = TurretCanon.rotation;
            Quaternion b = Quaternion.Euler(0, TurretCanon.rotation.y, 0);
            TurretCanon.rotation = Quaternion.Lerp(a, b, TurretBackRotateTime * Time.deltaTime);
        }
        private void FlipCanonTurret()
        {
            isFacingRight = !isFacingRight;
            Vector3 scaler = TurretCanon.localScale;
            scaler.y *= -1;
            TurretCanon.localScale = scaler;
        }
        protected EnemyAI GetNearestEnemy()
        {
            if (targets == null | targets.Count <= 0)
                return null;

            List<EnemyAI> enemyList = ConvertTargetsToEnemyList();
            EnemyAI enemy = enemyList[0];
            float curDistance = 1000f;

            for (int i = 1; i < enemyList.Count; i++)
            {
                float targetDistance = Vector2.Distance(transform.position, enemyList[i].transform.position);
                if (targetDistance < curDistance)
                {
                    enemy = enemyList[i];
                    curDistance = targetDistance;
                }
            }

            if (enemy != null)
            {
                enemyTrajectory = enemy.transform.position + (Vector3)enemy.rb.velocity / 2;
                return enemy;
            }
            else
            {
                currentEnemy = null;
                enemyTrajectory = Vector2.zero;
                return null;
            }
        }

        private List<EnemyAI> ConvertTargetsToEnemyList()
        {
            List<EnemyAI> enemyList = new List<EnemyAI>();
            List<int> errorIndex = new List<int>();

            for (int i = 0; i < targets.Count; i++)
            {
                if(targets[i] == null)
                {
                    errorIndex.Add(i);
                    continue;
                }

                if (targets[i].TryGetComponent(out EnemyAI enemyAi))
                {
                    enemyList.Add(enemyAi);
                }
                else
                {
                    errorIndex.Add(i);
                }
            }

            if (errorIndex.Count > 0)
            {
                foreach (var i in errorIndex)
                {
                    targets.RemoveAt(i);
                }
            }

            return enemyList;
        }
        #endregion

        #region ICraftableBuild
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        public bool CanPut()
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(targetPlacePosition, MendatoryFreeRadius, ConflictedLayers);
            if (colls.Length > 0)
            {
                return !(colls.Length == 1 && colls[0] != GetComponent<Collider2D>());
            }

            return true;
        }
        public void Put()
        {
            transform.position = targetPlacePosition;
            PreRenderPlaceObject.gameObject.SetActive(false);

            isPicked = false;
            EnemyDetectionCollider.enabled = true;

            _enemySpawner.AddTarget(EnemyTarget);
        }
        public bool CanPick()
            => true;
        #endregion

        #region Break
        public virtual void Break()
        {    
            player.Build.CleanPickedObject(this);

            var inventory = ServiceLocator.GetService<PlayerInventory>();
            foreach (var item in DroppedItems)
            {
                inventory.AddItem(item.Item, item.Amount);
            }

            _enemySpawner.RemoveTarget(EnemyTarget);
            Destroy(gameObject);
        }
        #endregion

        #region Attacking
        protected abstract void Attack();
        #endregion

        #region Health
        public virtual void TakeDamage(float value)
        {
            currentHealth -= value;

            if(currentHealth <= 0)
            {
                Die();
            }
        }
        [Button]
        protected virtual void Die()
        {    
            player.Build.CleanPickedObject(this);

            GameObject particle = Instantiate(DestroyParticle, transform.position, Quaternion.identity);
            particle.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));

            Destroy(gameObject);
        }

        void IDamagable.Damage(float dmg, EnemyTarget enemyTarget)
        {
            TakeDamage(dmg);
        }
        void IDamagable.Die()
        {
            _enemySpawner.RemoveTarget(EnemyTarget);
            Die();
        }
        #endregion
    }
}

