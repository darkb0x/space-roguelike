using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Game.Turret
{
    using Player;
    using Player.Inventory;
    using Enemy;

    public abstract class Turret : MonoBehaviour, IDamagable
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

        private PlayerController player;
        private Transform myTransform;

        [Header("Turret variables")]
        [SerializeField] protected GameObject BulletPrefab;
        [SerializeField] protected float Damage = 1;
        [SerializeField] protected float TimeBtwAttack = 0.3f;
        [SerializeField] protected float Recoil = 0f;
        [Space]
        [SerializeField] private List<DroppedItem> DroppedItems = new List<DroppedItem>(1);

        [Header("Turret survivability")]
        [SerializeField] private Enemy.EnemyTarget EnemyTarget;
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
        [ReadOnly] public Transform currentEnemyTransform;
        protected EnemyAI currentEnemy;
        protected Vector2 enemyTrajectory;
        [ReadOnly] public List<GameObject> targets = new List<GameObject>();

        [Header("Other")]
        [Tag, SerializeField] protected string PlayerTag = "Player";
        [field: SerializeField] public bool isPicked { get; protected set; }
        
        [Header("Collisions")]
        [SerializeField] protected CircleCollider2D EnemyDetectionCollider;
        [SerializeField] protected CircleCollider2D PlayerDetectionCollider;

        private void OnDrawGizmosSelected()
        {
            if (currentEnemyTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(myTransform.position, currentEnemyTransform.transform.position);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(currentEnemyTransform.position, currentEnemyTransform.position + (Vector3)currentEnemy.rb.velocity / 2);
            }
        }

        protected virtual void Start()
        {
            myTransform = GetComponent<Transform>();

            if(!ChangeRotationTime)
            {
                TurretRotateTime = STANDART_ROTATION_TIME;
                TurretBackRotateTime = STANDART_ROTATION_TIME;
            }

            EnemyTarget.Initialize(this);
        }

        public virtual void Initialize(PlayerController p)
        {
            player = p;

            currentTimeBtwAttacks = TimeBtwAttack;
            currentHealth = Health;

            isPicked = true;
            EnemyDetectionCollider.enabled = false;

            player.pickObjSystem.SetPickedGameobj(gameObject);
        }

        #region Updates
        protected virtual void Update()
        {
            if (Keyboard.current.kKey.isPressed)
                Die();

            if (isPicked)
            {
                return;
            }

            #region rotation
            if (currentEnemyTransform != null)
            {
                RotateToTarget(currentEnemyTransform);
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
                if (currentEnemyTransform == null)
                    currentEnemyTransform = GetNearestEnemy();

                if (currentTimeBtwAttacks <= 0)
                {
                    if (!isPicked)
                        Attack();
                    currentEnemyTransform = GetNearestEnemy();
                    currentTimeBtwAttacks = TimeBtwAttack;
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
            if (currentEnemyTransform != null)
            {
                Vector3 dir = Vector3.zero;
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
        protected virtual Transform GetNearestEnemy()
        {
            if (targets == null | targets.Count <= 0)
                return null;

            GameObject enemy = targets[0];
            float curDistance = 1000f;
            if (enemy != null)
                Vector2.Distance(myTransform.position, enemy.transform.position);
            int errorIndex = 0;
            for (int i = 1; i < targets.Count; i++)
            {
                if(targets[i] != null)
                {
                    float targetDistance = Vector2.Distance(myTransform.position, targets[i].transform.position);
                    if (targetDistance < curDistance)
                        enemy = targets[i];
                }
                else
                {
                    errorIndex = 0;
                    break;
                }
            }
            targets.RemoveAt(errorIndex);
            if (enemy != null)
            {
                currentEnemy = enemy.GetComponent<EnemyAI>();
                enemyTrajectory = enemy.transform.position + (Vector3)currentEnemy.rb.velocity / 2;
                return enemy.transform;
            }
            else
            {
                currentEnemy = null;
                enemyTrajectory = Vector2.zero;
                return null;
            }
        }
        #endregion

        public virtual void Put()
        {
            isPicked = false;
            EnemyDetectionCollider.enabled = true;

            EnemySpawner.Instance.AddTarget(EnemyTarget);
        }

        #region Break
        public virtual void Break()
        {
            if (isPicked)
            {
                player.pickObjSystem.PutCurrentGameobj(false);
            }

            foreach (var item in DroppedItems)
            {
                PlayerInventory.Instance.GiveItem(item.Item, item.Amount);
            }

            EnemySpawner.Instance.RemoveTarget(EnemyTarget);
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
            if(isPicked)
                player.pickObjSystem.PutCurrentGameobj(false);

            GameObject particle = Instantiate(DestroyParticle, myTransform.position, Quaternion.identity);
            particle.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));

            Destroy(gameObject);
        }

        void IDamagable.Damage(float dmg, Enemy.EnemyTarget enemyTarget)
        {
            TakeDamage(dmg);
        }
        void IDamagable.Die()
        {
            EnemySpawner.Instance.RemoveTarget(EnemyTarget);
            Die();
        }
        #endregion
    }
}

