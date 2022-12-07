using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Turret
{
    using Player;
    using Player.Inventory;
    using Bullets;

    public abstract class TurretAI : MonoBehaviour
    {
        [System.Serializable]
        public struct Item
        {
            public InventoryItem item;
            public int amount;
        }

        [Header("Turret firpower parameters")]
        public Transform shotPos;
        [Space]
        public GameObject bulletPrefab;
        public float damage = 1;
        public float timeBtwAttack = 0.3f;
        public float recoil = 0f;

        [Header("Turret rotation")]
        public float turret_rotateTime = 0.2f;
        public float turret_back_RotateTime = 0.05f;
        public Transform turret_canon;

        [Header("Enemy detecion")]
        [Tag] public string enemyTag;
        [Space]
        [ReadOnly] public bool enemyInZone;
        [ReadOnly] public GameObject currentEnemy;
        [ReadOnly] public List<GameObject> targets = new List<GameObject>();

        [Header("other")]
        public bool isPicked;
        public Collider2D coll;
        public List<Item> droppedItems = new List<Item>();

        PlayerController player;
        PlayerInventory inventory;
        new Transform transform;
        [HideInInspector] public float currentTimeBtwAttacks;

        private void OnDrawGizmosSelected()
        {
            if (currentEnemy != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, currentEnemy.transform.position);
            }
        }

        public virtual void Start()
        {
            player = FindObjectOfType<PlayerController>();
            inventory = FindObjectOfType<PlayerInventory>();
            transform = GetComponent<Transform>();

            currentTimeBtwAttacks = timeBtwAttack;

            //coll.enabled = false;
        }

        #region Updates
        private void Update()
        {
            if (currentEnemy != null)
            {
                RotateToTarget(currentEnemy);
            }
            else
            {
                GetRotateBack();
            }
            enemyInZone = (targets.Count > 0);

            if (!isPicked) Run();
        }
        private void FixedUpdate()
        {
            if (!isPicked) FixedRun();
        }

        public virtual void Run() 
        {
            if (!enemyInZone)
                return;

            if(currentTimeBtwAttacks <= 0)
            {
                Attack();
                currentTimeBtwAttacks = timeBtwAttack;
            }
            else
            {
                currentTimeBtwAttacks -= Time.deltaTime;
            }
        }
        public virtual void FixedRun() { }
        #endregion

        #region Collision triggers
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == enemyTag)
            {
                if (!targets.Contains(collision.gameObject))
                    targets.Add(collision.gameObject);
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == enemyTag)
            {
                if (currentEnemy == null)
                {
                    currentEnemy = GetNearestEnemy();
                }
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == enemyTag)
            {
                targets.Remove(collision.gameObject);
                if (currentEnemy == collision.gameObject)
                {
                    currentEnemy = GetNearestEnemy();
                }
            }
        }
        #endregion

        #region Utilties
        private void RotateToTarget(GameObject target)
        {
            if (currentEnemy != null)
            {
                Vector3 dir = target.transform.position - turret_canon.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion a = turret_canon.rotation;
                Quaternion b = Quaternion.Euler(0, 0, angle);
                turret_canon.rotation = Quaternion.Lerp(a, b, turret_rotateTime);
            }
            else
            {
                return;
            }
        }
        private void GetRotateBack()
        {
            Quaternion a = turret_canon.rotation;
            Quaternion b = Quaternion.Euler(0, 0, 0);
            turret_canon.rotation = Quaternion.Lerp(a, b, turret_back_RotateTime);
        }
        public GameObject GetNearestEnemy()
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
            return enemy;
        }
        #endregion

        public virtual void Attack()
        {
            float recoil_rotation = Random.Range(-recoil, recoil);
            Bullet bullet = Instantiate(bulletPrefab, shotPos.position, shotPos.rotation).GetComponent<Bullet>();
            bullet.gameObject.transform.Rotate(0, 0, recoil_rotation);
            bullet.Init(damage);
        }

        private void Put()
        {
            isPicked = false;
            coll.enabled = true;
        }

        public virtual void DestroyTurret()
        {
            foreach (var item in droppedItems)
            {
                inventory.AddItem(item.item, item.amount);
            }
            Destroy(gameObject);
        }
    }
}
