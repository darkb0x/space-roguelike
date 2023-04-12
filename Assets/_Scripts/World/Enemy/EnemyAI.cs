using System.Collections;
using System.Collections.Generic;
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, VisionRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRadius);
        }

        public virtual void Start()
        {
            seecker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            myTransform = transform;

            HealthBarObject.SetActive(false);

            shieldTime = m_ShieldTime;
            haveShield = true;

            if(InitializeOnStart)
                Initialize(Data);
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
            InvokeRepeating("CheckTargetsInVision", 0, CHECK_TARGET_IN_VISION_TIME);
        }

        public virtual void Update()
        {
            #if UNITY_EDITOR
            if (Keyboard.current.kKey.isPressed)
                Die();
            #endif

            if(shieldTime > 0)
            {
                shieldTime -= Time.deltaTime;
            }
            else
            {
                haveShield = false;
            }

            if (currentTarget == null)
            {
                if(!targetInVision)
                {
                    currentTarget = GetRandomTarget();
                }
            }

            if (reachedEndOfPath)
            {
                if(IsAttacking)
                {
                    if (currentTimeBtwAttack <= 0)
                    {
                        currentTimeBtwAttack = TimeBtwAttacks;

                        StartAttacking();
                    }
                    else
                    {
                        currentTimeBtwAttack -= Time.deltaTime;
                    }
                }
            }
        }
        public virtual void FixedUpdate()
        {
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

            float speedFactor = 50f;
            Vector2 force = direction * (Speed * speedFactor) * Time.deltaTime;
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
        private void StartAttacking()
        {
            EnemyVisual.StartAttacking();
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
            EnemyTarget[] targets = EnemySpawner.Instance.GetTargetList();

            if (targets.Length <= 0)
                return null;

            return targets[Random.Range(0, targets.Length)];
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

            EnemySpawner.Instance.RemoveEnemy(this);

            foreach (var coll in AllColls)
            {
                coll.enabled = false;
            }

            this.enabled = false;
        }
        #endregion
    }
}
