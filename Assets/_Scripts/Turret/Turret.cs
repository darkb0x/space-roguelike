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
    using AI;

    public class Turret : MonoBehaviour, IDamagable
    {
        [System.Serializable]
        public struct Item
        {
            public InventoryItem item;
            public int amount;
        }
        private bool isFacingRight = true;
        private float currentBreakProgress;
        private PlayerController player;
        private bool playerInZone = false;

        [Header("Turret data")]
        [Expandable] public TurretData Data;

        [Header("Turret AI")]
        [Expandable] public TurretAI AI;

        [Header("Turret")]
        public float TurretRotateTime = 0.2f;
        public float TurretBackRotateTime = 0.05f;
        [Space]
        public Transform TurretCanon;
        public Transform ShotPos;

        [Header("Turret survivability")]
        [ReadOnly] public float currentHealth;
        [SerializeField] private Enemy.EnemyTarget EnemyTarget;

        [Header("Enemy detecion")]
        [Tag] public string EnemyTag;
        [Space]
        [ReadOnly] public bool enemyInZone;
        [ReadOnly] public Transform currentEnemy;
        [ReadOnly] public List<GameObject> targets = new List<GameObject>();

        [Header("Break system")]
        public GameObject breakProgress_gameObj;
        public Image breakProgress_image;
        public float breakTime;

        [Header("Renderer")]
        public SpriteRenderer BodySpriteRenderer;
        public SpriteRenderer CanonSpriteRenderer;

        [Header("Other")]
        [Tag] public string PlayerTag = "Player";
        public bool isPicked;
        
        [Header("Collisions")]
        public CircleCollider2D EnemyDetectionCollider;
        public CircleCollider2D PlayerDetectionCollider;

        new Transform transform;
        [HideInInspector] public float currentTimeBtwAttacks;

        private void OnDrawGizmosSelected()
        {
            if (currentEnemy != null)
            {
                Gizmos.DrawWireCube(currentEnemy.transform.position, Vector3.one*1.5f);
            }
            if(targets.Count > 0)
            {
                foreach (var target in targets)
                {
                    if(target.transform == GetNearestEnemy())
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(transform.position, currentEnemy.transform.position);

                        continue;
                    }

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, target.transform.position);
                }
            }

            if(AI != null)
            {
                AI.turret = this;

                if (ShotPos == null | TurretCanon == null)
                    return;
                if (Data == null)
                    return;

                if (AI is TurretLaser)
                {
                    float distance = (float)Data.GetVariable(TurretLaser.LASER_DISTANCE);

                    Debug.DrawRay(ShotPos.position, TurretCanon.right * distance, Color.blue);
                }
                if(AI is TurretFiregun)
                {
                    float distance = (float)Data.GetVariable(TurretFiregun.DISTANCE);
                    float range = (float)Data.GetVariable(TurretFiregun.RANGE);

                    Debug.DrawRay(ShotPos.position, TurretCanon.right * distance, Color.blue);
                    Debug.DrawRay(ShotPos.position, (TurretCanon.right + TurretCanon.up * range) * distance, Color.blue);
                    Debug.DrawRay(ShotPos.position, (TurretCanon.right + TurretCanon.up * -range) * distance, Color.blue);
                }
            }
        }

        public void Start()
        {
            transform = GetComponent<Transform>();

            currentBreakProgress = breakTime;
            EnemyTarget.Initialize(this);

            breakProgress_gameObj.SetActive(false);
        }

        public void Initialize(PlayerController p, TurretAI ai, TurretData data)
        {
            player = p;

            AI = Instantiate(ai);
            AI.turret = this;
            AI.data = data;
            AI.Initialize();

            Data = data;
            if(Data._bodySprite == null | Data._canonSprite == null)
            {
                Debug.LogWarning($"{gameObject.name}/Turret.cs/Data({Data.name}) _bodySprite or _canonSprite is null");
            }
            else
            {
                BodySpriteRenderer.sprite = Data._bodySprite;
                CanonSpriteRenderer.sprite = Data._canonSprite;
            }

            currentTimeBtwAttacks = Data._timeBtwAttack;
            EnemyDetectionCollider.radius = Data._colliderSize;
            currentHealth = Data._health;

            isPicked = true;
            EnemyDetectionCollider.enabled = false;

            player.pickObjSystem.SetPickedGameobj(gameObject);
        }

        #region Updates
        private void Update()
        {
            AI.Run();

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

            enemyInZone = (targets.Count > 0);

            if(playerInZone)
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
        public Transform GetNearestEnemy()
        {
            if (targets == null | targets.Count <= 0)
                return null;

            GameObject enemy = targets[0];
            float curDistance = Vector2.Distance(transform.position, enemy.transform.position);
            for (int i = 1; i < targets.Count; i++)
            {
                if (Vector2.Distance(transform.position, targets[i].transform.position) < curDistance)
                    enemy = targets[i];
            }
            return enemy.transform;
        }
        #endregion

        public void Put()
        {
            isPicked = false;
            EnemyDetectionCollider.enabled = true;
        }

        #region Break
        public void DoBreak()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                breakProgress_gameObj.SetActive(true);
                breakProgress_image.fillAmount = 0f;
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
                    breakProgress_image.fillAmount = Mathf.Abs((currentBreakProgress / breakTime) - 1);
                }
            }
            if (Input.GetKeyUp(KeyCode.R))
            {
                StartCoroutine(EndBreaking());
            }
        }
        public IEnumerator EndBreaking()
        {
            while (breakProgress_image.fillAmount >= 0.01f)
            {
                currentBreakProgress = Mathf.Lerp(currentBreakProgress, breakTime, 0.2f); ;

                breakProgress_image.fillAmount = Mathf.Abs((currentBreakProgress / breakTime) - 1);

                yield return null;
            }

            breakProgress_gameObj.SetActive(false);
        }

        public virtual void Break()
        {
            if (isPicked)
            {
                player.pickObjSystem.PutCurrentGameobj(false);
            }

            foreach (var item in Data._droppedItems)
            {
                PlayerInventory.instance.GiveItem(item.Item, item.Amount);
            }

            EnemySpawner.instance.RemoveTarget(EnemyTarget);
            Destroy(gameObject);
        }
        #endregion

        #region Health
        public void TakeDamage(float value)
        {
            currentHealth -= value;

            if(currentHealth <= 0)
            {
                Die();
            }
        }
        private void Die()
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

