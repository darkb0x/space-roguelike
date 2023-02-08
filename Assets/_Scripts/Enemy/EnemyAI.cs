using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using NaughtyAttributes;

namespace Game.Enemy
{
    public abstract class EnemyAI : MonoBehaviour
    {
        private const float UPDATE_PATH_TIME = 1f;
        private const float CHECK_TARGET_IN_VISION_TIME = 0.7f;

        [Header("Health")]
        public float MaxHp;
        [ReadOnly] public float currentHp;
        [Space]
        [ReadOnly] public float currentProtection;
        public float Protection;
        [Space]
        [SerializeField] private GameObject HealthBarObject;
        [SerializeField] private Image HealthBarImage;

        [Header("Attack")]
        public bool IsAttacking = true;
        [Space]
        public float Damage;
        public float TimeBtwAttacks;
        [Space]
        [SerializeField] private float VisionRadius;
        [SerializeField] private LayerMask TargetLayer;

        [Header("Movement")]
        public float Speed;
        [SerializeField] private float NextWaypointDistance = 2f;

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

        // Components
        protected Transform myTransform;
        protected Seeker seecker;
        protected Rigidbody2D rb;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, VisionRadius);
        }

        public virtual void Start()
        {
            currentHp = MaxHp;
            currentTimeBtwAttack = TimeBtwAttacks;

            HealthBarObject.SetActive(false);

            seecker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            myTransform = transform;

            currentTarget = GetRandomTarget();

            InvokeRepeating("UpdatePath", 0f, UPDATE_PATH_TIME);
            InvokeRepeating("CheckTargetsInVision", 0, CHECK_TARGET_IN_VISION_TIME);
        }

        public virtual void Update()
        {
            if (!IsAttacking)
                return;

            if (currentTarget == null)
            {
                if(!targetInVision)
                {
                    currentTarget = GetRandomTarget();
                }
            }

            if (reachedEndOfPath)
            {
                if(currentTimeBtwAttack <= 0)
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
            Vector2 force = direction * (Speed * 50f) * Time.deltaTime;

            rb.AddForce(force);

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
        public virtual void Attack()
        {
            currentTarget.Hurt(Damage);
        }

        protected virtual void CheckTargetsInVision()
        {
            Collider2D[] allTargetsInVision = Physics2D.OverlapCircleAll(myTransform.position, VisionRadius, TargetLayer);
            foreach (var enemyTarget in allTargetsInVision)
            {
                if(enemyTarget.TryGetComponent<EnemyTarget>(out EnemyTarget target))
                {
                    targetInVision = true;
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
            EnemyTarget[] targets = EnemySpawner.instance.GetTargetList().ToArray();

            if (targets.Length <= 0)
                return null;

            return targets[Random.Range(0, targets.Length - 1)];
        }
        #endregion

        #region Hurt
        public virtual void TakeDamage(float value)
        {
            float dmg = value - currentProtection;

            if (dmg <= 0)
            {
                Protection--;
                if (Protection <= 0)
                {
                    currentProtection = 0;
                }
            }
            else
            {
                currentHp = Mathf.Clamp(currentHp - dmg, 0, MaxHp);
            }

            if (currentHp < MaxHp)
            {
                HealthBarObject.SetActive(true);
                HealthBarImage.fillAmount = currentHp / MaxHp;
            }

            if (currentHp <= 0)
                Die();
        }

        public virtual void Die()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
