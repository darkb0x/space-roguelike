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
        public float AttackRadius;
        public float TimeBtwAttacks;
        public LayerMask AttackLayers;
        [Space]
        public float VisionRadius;
        public LayerMask TargetLayer;

        [Header("Movement")]
        public float Speed;
        public float NextWaypointDistance = 2f;

        // Movement variables
        private Path path;
        private int currentWaypoint = 0;
        protected bool reachedEndOfPath = false;

        // Attack variables
        protected EnemyTarget currentTarget;
        protected float currentTimeBtwAttack;
        private bool targetInVision = false;

        // Components
        protected Transform myTransform;
        protected Seeker seecker;
        protected Rigidbody2D rb;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRadius);

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

                    Attack();
                }
                else
                {
                    currentTimeBtwAttack -= Time.deltaTime;
                }
            }
        }
        public virtual void FixedUpdate()
        {
            if (IsAttacking)
                Movement();
        }

        #region Move
        protected virtual void Movement()
        {
            if (path == null)
            {
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
            Vector2 force = direction * (Speed * 100f) * Time.deltaTime;
            rb.AddForce(force, ForceMode2D.Force);
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
        protected virtual void Attack()
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
                }
            }
            targetInVision = false;
        }
        protected virtual EnemyTarget GetRandomTarget()
        {
            EnemyTarget[] targets = FindObjectsOfType<EnemyTarget>();
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
