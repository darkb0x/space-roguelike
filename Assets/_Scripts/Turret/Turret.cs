using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

namespace Game.Turret
{
    using Player;
    using Player.Inventory;
    using Bullets;

    public abstract class Turret : MonoBehaviour, IDamagable
    {
        [System.Serializable]
        public struct DroppedItem
        {
            public InventoryItem Item;
            public int Amount;
        }
        private bool isFacingRight = true;
        protected bool playerInZone { get; private set; }
        protected bool enemyInZone { get; private set; }

        private float currentBreakProgress;
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

        [Header("Turret/Canon")]
        [SerializeField] protected float TurretRotateTime = 0.2f;
        [SerializeField] protected float TurretBackRotateTime = 0.05f;
        [Space]
        [SerializeField] protected Transform TurretCanon;
        [SerializeField] protected Transform ShotPos;


        [Header("Enemy detecion")]
        [Tag, SerializeField] protected string EnemyTag = "Enemy";
        [Space]
        [ReadOnly] public Transform currentEnemy;
        [ReadOnly] public List<GameObject> targets = new List<GameObject>();

        [Header("Break system")]
        [SerializeField] private GameObject BreakProgressGameObj;
        [SerializeField] private Image BreakProgressImage;
        [SerializeField] protected float BreakTime = 5;

        [Header("Other")]
        [Tag, SerializeField] protected string PlayerTag = "Player";
        [field: SerializeField] public bool isPicked { get; protected set; }
        
        [Header("Collisions")]
        [SerializeField] protected CircleCollider2D EnemyDetectionCollider;
        [SerializeField] protected CircleCollider2D PlayerDetectionCollider;

        private void OnDrawGizmosSelected()
        {
            if (currentEnemy != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(myTransform.position, currentEnemy.transform.position);
                Gizmos.DrawWireSphere(currentEnemy.transform.position, 1.5f);
            }
        }

        protected virtual void Start()
        {
            myTransform = GetComponent<Transform>();

            currentBreakProgress = BreakTime;
            EnemyTarget.Initialize(this);

            BreakProgressGameObj.SetActive(false);

            Initialize(FindObjectOfType<PlayerController>());
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
            if (isPicked)
            {
                DoBreak();

                return;
            }

            #region rotation
            if (currentEnemy != null)
            {
                RotateToTarget(currentEnemy);
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

            currentEnemy = GetNearestEnemy();

            if(enemyInZone)
            {
                if (currentTimeBtwAttacks <= 0)
                {
                    Attack();
                    currentTimeBtwAttacks = TimeBtwAttack;
                }
                else
                {
                    currentTimeBtwAttacks -= Time.deltaTime;
                }
            }
            #endregion

            if (playerInZone)
            {
                DoBreak();
            }
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
                Vector3 dir = target.position - TurretCanon.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion a = TurretCanon.rotation;
                Quaternion b = Quaternion.Euler(0, TurretCanon.rotation.y, angle);
                TurretCanon.rotation = Quaternion.Lerp(a, b, TurretRotateTime);
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
            TurretCanon.rotation = Quaternion.Lerp(a, b, TurretBackRotateTime);
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
            float curDistance = Vector2.Distance(myTransform.position, enemy.transform.position);
            for (int i = 1; i < targets.Count; i++)
            {
                if (Vector2.Distance(myTransform.position, targets[i].transform.position) < curDistance)
                    enemy = targets[i];
            }
            return enemy.transform;
        }
        #endregion

        public virtual void Put()
        {
            isPicked = false;
            EnemyDetectionCollider.enabled = true;

            EnemySpawner.instance.AddTarget(EnemyTarget);
        }

        #region Break
        protected void DoBreak()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                BreakProgressGameObj.SetActive(true);
                BreakProgressImage.fillAmount = 0f;
            }
            if (Input.GetKey(KeyCode.R))
            {
                if (currentBreakProgress <= 0)
                {
                    Break();
                }
                else
                {
                    currentBreakProgress -= Time.deltaTime;
                    BreakProgressImage.fillAmount = Mathf.Abs((currentBreakProgress / BreakTime) - 1);
                }
            }
            if (Input.GetKeyUp(KeyCode.R))
            {
                StartCoroutine(EndBreaking());
            }
        }
        protected IEnumerator EndBreaking()
        {
            while (BreakProgressImage.fillAmount >= 0.01f)
            {
                currentBreakProgress = Mathf.Lerp(currentBreakProgress, BreakTime, 0.2f); ;

                BreakProgressImage.fillAmount = Mathf.Abs((currentBreakProgress / BreakTime) - 1);

                yield return null;
            }

            BreakProgressGameObj.SetActive(false);
        }

        protected virtual void Break()
        {
            if (isPicked)
            {
                player.pickObjSystem.PutCurrentGameobj(false);
            }

            foreach (var item in DroppedItems)
            {
                PlayerInventory.instance.GiveItem(item.Item, item.Amount);
            }

            EnemySpawner.instance.RemoveTarget(EnemyTarget);
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
        protected virtual void Die()
        {
            Destroy(gameObject);
        }

        void IDamagable.Damage(float dmg, Enemy.EnemyTarget enemyTarget)
        {
            TakeDamage(dmg);
        }
        void IDamagable.Die()
        {
            EnemySpawner.instance.RemoveTarget(EnemyTarget);
            Die();
        }
        #endregion
    }
}

