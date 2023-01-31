using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Turret
{
    using Player;
    using Player.Inventory;
    using Bullets;
    using AI;

    public class Turret : MonoBehaviour
    {
        [System.Serializable]
        public struct Item
        {
            public InventoryItem item;
            public int amount;
        }
        private bool isFacingRight = true;

        [Header("Turret data")]
        [Expandable] public TurretData Data;

        [Header("Turret AI")]
        [Expandable] public TurretAI AI;

        [Header("Turret rotation")]
        public float TurretRotateTime = 0.2f;
        public float TurretBackRotateTime = 0.05f;
        public Transform TurretCanon;

        [Header("Enemy detecion")]
        [Tag] public string EnemyTag;
        [Space]
        [ReadOnly] public bool enemyInZone;
        [ReadOnly] public Transform currentEnemy;
        [ReadOnly] public List<GameObject> targets = new List<GameObject>();

        [Header("other")]
        public Transform shotPos;
        public bool isPicked;
        public CircleCollider2D coll;

        new Transform transform;
        [HideInInspector] public float currentTimeBtwAttacks;

        private TurretAI currentAI;

        private void OnDrawGizmosSelected()
        {
            if (currentEnemy != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, currentEnemy.transform.position);

                Gizmos.DrawWireCube(currentEnemy.transform.position, Vector3.one*1.5f);
            }

            if(AI != null)
            {
                AI.turret = this;

                if (shotPos == null | TurretCanon == null)
                    return;
                if (Data == null)
                    return;

                if (AI is TurretLaser)
                {
                    float distance = (float)Data.GetVariable(TurretLaser.LASER_DISTANCE);

                    Debug.DrawRay(shotPos.position, TurretCanon.right * distance, Color.blue);
                }
                if(AI is TurretFiregun)
                {
                    float distance = (float)Data.GetVariable(TurretFiregun.DISTANCE);
                    float range = (float)Data.GetVariable(TurretFiregun.RANGE);

                    Debug.DrawRay(shotPos.position, TurretCanon.right * distance, Color.blue);
                    Debug.DrawRay(shotPos.position, (TurretCanon.right + TurretCanon.up * range) * distance, Color.blue);
                    Debug.DrawRay(shotPos.position, (TurretCanon.right + TurretCanon.up * -range) * distance, Color.blue);
                }
            }
        }

        public void Start()
        {
            transform = GetComponent<Transform>();
        }

        public void Initialize(PlayerController p, TurretAI ai, TurretData data)
        {
            currentAI = ai;
            currentAI = Instantiate(AI);
            currentAI.turret = this;
            currentAI.Initialize();

            Data = data;

            currentTimeBtwAttacks = Data._timeBtwAttack;
            coll.radius = Data._colliderSize;

            isPicked = true;
            coll.enabled = false;

            p.pickObjSystem.SetPickedGameobj(gameObject);
        }

        #region Updates
        private void Update()
        {
            currentAI.Run();

            if (isPicked)
                return;

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
        }
        #endregion

        #region Collision triggers
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == EnemyTag)
            {
                if (!targets.Contains(collision.gameObject))
                    targets.Add(collision.gameObject);
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == EnemyTag)
            {
                currentEnemy = GetNearestEnemy();
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == EnemyTag)
            {
                targets.Remove(collision.gameObject);
                if (currentEnemy == collision.transform)
                {
                    currentEnemy = GetNearestEnemy();
                }
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
            coll.enabled = true;
        }
    }
}

