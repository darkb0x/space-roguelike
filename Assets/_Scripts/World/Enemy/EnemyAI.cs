using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Pathfinding;
using NaughtyAttributes;

namespace Game.Enemy
{
    public abstract class EnemyAI : MonoBehaviour
    {
        private const float UPDATE_PATH_TIME = 0.3f;
        private const float CHECK_TARGET_IN_VISION_TIME = 0.7f;

        [Header("Data")]
        [SerializeField, Expandable] protected EnemyData Data;
        [SerializeField] private bool InitializeOnStart = false;

        [Header("Health")]
        [SerializeField] private float m_ShieldTime = 0.7f;
        [Space]
        [ReadOnly] public float currentHp;
        [ReadOnly] public float maxHp;
        [Space]
        [ReadOnly] public float currentProtection;
        [Space]
        [SerializeField] private GameObject HealthBarObject;
        [SerializeField] private Image HealthBarImage;

        [Header("Attack")]
        public bool IsAttacking = true;
        [Space]
        [SerializeField] protected float AttackRadius;
        [ReadOnly] public float Damage;
        public float TimeBtwAttacks;
        [Space]
        [SerializeField] private float VisionRadius;
        [SerializeField] private LayerMask TargetLayer;

        [Header("Movement")]
        public float Speed;
        [SerializeField] private float NextWaypointDistance = 2f;
        [SerializeField] private Collider2D[] AllColls;

        [Header("Visual")]
        public EnemyVisual EnemyVisual;

        // Movement variables
        private Path path;
        private int currentWaypoint = 0;
        public bool reachedEndOfPath { get; protected set; }

        // Attack variables
        public EnemyTarget currentTarget { get; protected set; }
        protected float currentTimeBtwAttack;
        private bool targetInVision = false;
        protected float shieldTime;
        protected bool haveShield = true;

        // Components
        protected Transform myTransform;
        protected Seeker seecker;
        public Rigidbody2D rb { get; private set; }

        private EnemySpawner EnemySpawner;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, VisionRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRadius);
        }

        public virtual void Start()
        {
            EnemySpawner = Singleton.Get<EnemySpawner>();

            EnemySpawner.OnTargetRemoved += OnTargetRemoved;

            seecker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            myTransform = transform;

            HealthBarObject.SetActive(false);

            shieldTime = m_ShieldTime;
            haveShield = true;

            if(InitializeOnStart)
                Initialize(Data);
        }

        private void OnDisable()
        {
            EnemySpawner.OnTargetRemoved -= OnTargetRemoved;
        }

        public virtual void Initialize(EnemyData data, float difficultFactor = 1)
        {
            Data = data;

            currentHp = Data.Health * difficultFactor;
            maxHp = Data.Health;
            currentProtection = Data.Protection * difficultFactor;
            Damage = Data.Damage;
            currentTimeBtwAttack = TimeBtwAttacks;

            currentTarget = GetRandomTarget();

            InvokeRepeating("UpdatePath", 0f, UPDATE_PATH_TIME);
            InvokeRepeating("CheckTargetsInVision", 0, CHECK_TARGET_IN_VISION_TIME);
        }
        public virtual void Initialize(float health, float protection, float damage, bool isAttacking, float difficultFactor = 1)
        {
            currentHp = health * difficultFactor;
            maxHp = currentHp;
            currentProtection = protection * difficultFactor;
            Damage = damage;
            currentTimeBtwAttack = TimeBtwAttacks;
            this.IsAttacking = isAttacking;

            currentTarget = GetRandomTarget();

            InvokeRepeating("UpdatePath", 0f, UPDATE_PATH_TIME);
            InvokeRepeating("CheckTargetsInVision", 0f, CHECK_TARGET_IN_VISION_TIME);
        }

        public virtual void Update()
        {
            // Start shield
            if(shieldTime > 0)
            {
                shieldTime -= Time.deltaTime;
            }
            else
            {
                haveShield = false;
            }

            // Target selection
            if (currentTarget == null)
            {
                if(!targetInVision)
                {
                    currentTarget = GetRandomTarget();
                }
            }

            // Attacking
            TryAttack();
        }
        public virtual void FixedUpdate()
        {
            // Movement
            if (IsAttacking && !reachedEndOfPath)
                Movement();
        }

        #region Move
        protected virtual void Movement()
        {
            if (path == null)
            {
                reachedEndOfPath = true;
                return;
            }
            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            float forceFactor = 1;
            float minDistance = 5f;

            if (Vector2.Distance(myTransform.position, currentTarget.transform.position) <= minDistance)
            {
                float dotProduct = Vector2.Dot(currentTarget.GetMoveDirection(), rb.velocity.normalized);

                if(dotProduct < 0)
                {
                    forceFactor = 2f;
                    direction = (((Vector2)currentTarget.transform.position + (Vector2)currentTarget.GetMoveDirection()) - rb.position).normalized;
                }
            }

            float speedFactor = 50f;
            Vector2 force = direction * (Speed * speedFactor) * forceFactor * Time.fixedDeltaTime;

            rb.AddForce(force);

            //rb.MovePosition(rb.position + direction * Speed * Time.fixedDeltaTime);

            if(EnemyVisual)
                EnemyVisual.FlipSprite(rb.velocity.x < 0.2f);
            else
                Debug.LogWarning(gameObject.name + "/EnemyAI.cs/EnemyVisual is null!");

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < NextWaypointDistance)
            {
                currentWaypoint++;
            }
        }
        protected virtual void UpdatePath()
        {
            if (seecker.IsDone())
            {
                reachedEndOfPath = false;

                if (currentTarget == null)
                    return;
                seecker.StartPath(rb.position, currentTarget.transform.position, OnPathComlete);
            }
        }
        protected virtual void OnPathComlete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }
        #endregion

        #region Attack
        private void TryAttack()
        {
            if (!IsAttacking)
                return;
            if (currentTarget == null)
                return;

            float minDistanceForAttack = 2f;
            if (Vector2.Distance(myTransform.position, currentTarget.transform.position) <= minDistanceForAttack)
            {
                if (currentTimeBtwAttack <= 0)
                {
                    currentTimeBtwAttack = TimeBtwAttacks;

                    EnemyVisual.StartAttacking();
                }
                else
                {
                    currentTimeBtwAttack -= Time.deltaTime;
                }
            }
        }
        public virtual bool Attack()
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(myTransform.position, AttackRadius, TargetLayer);
            foreach (var target in targets)
            {
                if(target.TryGetComponent<EnemyTarget>(out EnemyTarget enemyTarget))
                {
                    if(enemyTarget == currentTarget)
                    {
                        currentTarget.Hurt(Damage);
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual void CheckTargetsInVision()
        {
            Collider2D[] allTargetsInVision = Physics2D.OverlapCircleAll(myTransform.position, VisionRadius, TargetLayer);
            foreach (var enemyTarget in allTargetsInVision)
            {
                if(enemyTarget.TryGetComponent<EnemyTarget>(out EnemyTarget target))
                {
                    if(!EnemySpawner.GetTargetList().Contains(target))
                    {
                        continue;
                    }

                    targetInVision = true;

                    if (currentTarget == null)
                    {
                        currentTarget = target;
                        continue;
                    }

                    if ((int)target.Priority > (int)currentTarget.Priority)
                    {
                        currentTarget = target;
                        break;
                    }
                    else
                    {
                        float fromEnemyTo_CurrentTarget = Vector2.Distance(myTransform.position, currentTarget.transform.position);
                        float fromEnemyTo_Target = Vector2.Distance(myTransform.position, target.transform.position);

                        if(fromEnemyTo_Target < fromEnemyTo_CurrentTarget)
                        {
                            currentTarget = target;
                            break;
                        }
                    }
                }
            }
            targetInVision = false;
        }
        protected virtual EnemyTarget GetRandomTarget()
        {
            EnemyTarget[] targets = Singleton.Get<EnemySpawner>().GetTargetList().ToArray();

            if (targets.Length <= 0)
                return null;

            return targets[Random.Range(0, targets.Length)];
        }

        private void OnTargetRemoved(EnemyTarget target)
        {
            if(currentTarget == target)
            {
                currentTarget = null;
            }
        }
        #endregion

        #region Hurt
        public virtual void TakeDamage(float value)
        {
            if (haveShield)
                return;

            float dmg = value - currentProtection;

            if (dmg <= 0)
            {
                currentProtection--;
                if (currentProtection <= 0)
                {
                    currentProtection = 0;
                }
            }
            else
            {
                currentHp = Mathf.Clamp(currentHp - dmg, 0, maxHp);
            }

            if (currentHp < maxHp)
            {
                HealthBarObject.SetActive(true);
                HealthBarImage.fillAmount = currentHp / maxHp;
            }

            if (currentHp <= 0)
                Die();
        }

        public virtual void Die(bool immediate = false)
        {
            if (haveShield && !immediate)
                return;

            IsAttacking = false;

            EnemyVisual.Death();

            EnemySpawner.RemoveEnemy(this);

            foreach (var coll in AllColls)
            {
                coll.enabled = false;
            }

            this.enabled = false;
        }
        #endregion
    }
}
