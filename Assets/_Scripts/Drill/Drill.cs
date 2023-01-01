using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Drill
{
    using World;
    using Player;
    using Player.Inventory;

    public abstract class Drill : MonoBehaviour, IMouseObserver_Click
    {
        [HideInInspector] public Transform myTransform;
        [HideInInspector] public bool isPicked = true;
        [HideInInspector] public Transform oreTransform;
        [HideInInspector] public PlayerController player;

        [Header("Variables")]
        public int damage;
        public float timeBtwMining;
        [HideInInspector] public float currentTimeBtwMining;
        [ReadOnly] public bool isMining = false;
        [ReadOnly] public int allExtractedOre = 0;
        [Space]
        [ReadOnly] public float health;
        public float maxHealth;
        [Space]
        public float oreFindingRadius = 1.2f;
        [HorizontalLine(color: EColor.Green)]
        public LayerMask oreLayer;
        [Space]
        public float pickOreFromInventoryMaxDistance = 3.4f;
        [HorizontalLine(color: EColor.Indigo)]

        [Header("Inventory")]
        public InventoryItem item;
        public int amount;
        [Space]
        [ReadOnly] public Ore currentOre;

        [Header("Components")]
        public Collider2D coll;
        [Space]
        public Animator anim;
        [AnimatorParam("anim")] public string anim_putTrigger;
        [AnimatorParam("anim")] public string anim_miningBool;
        [Space]
        public SpriteRenderer backLegsSR;
        [SortingLayer] public string worldSortingLayer;

        private void OnDrawGizmos()
        {
            // oreFindingRadius
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, oreFindingRadius);
            if (pickOreFromInventoryMaxDistance <= 0)
                Debug.LogWarning(gameObject.name + " oreFindingRadius is less or equals 0");

            // pickOreFromInventoryMaxDistance
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, pickOreFromInventoryMaxDistance);
            if (pickOreFromInventoryMaxDistance <= 0)
                Debug.LogWarning(gameObject.name + " pickOreFromInventoryMaxDistance is less or equals 0");
        }

        public virtual void Start()
        {
            player = FindObjectOfType<PlayerController>();
            myTransform = transform;

            currentTimeBtwMining = timeBtwMining;
            health = maxHealth;

            Initialize();
        }

        public virtual void Initialize()
        {
            isPicked = true;

            coll.enabled = false;

            player.pickObjSystem.SetPickedGameobj(gameObject);
        }

        private void Update()
        {
            if (isPicked)
                return;

            if(currentOre && isMining)
            {
                anim.SetBool(anim_miningBool, true);

                if(currentTimeBtwMining <= 0)
                {
                    Mine();

                    currentTimeBtwMining = timeBtwMining;
                }
                else
                {
                    currentTimeBtwMining -= Time.deltaTime;
                }
            }
            else
            {
                anim.SetBool(anim_miningBool, false);
            }
        }

        #region Put
        public bool CanPut()
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(myTransform.position, oreFindingRadius, oreLayer);
            foreach (var obj in colls)
            {
                if(obj.TryGetComponent<Ore>(out Ore ore))
                {
                    if (ore.currentDrill)
                        continue;
                    if (!ore.canGiveOre)
                        continue;

                    currentOre = ore;
                    oreTransform = ore.transform;

                    currentOre.currentDrill = this;
                    item = currentOre.item;

                    return true;
                }
            }
            return false;
        }
        public virtual void Put()
        {
            myTransform.position = oreTransform.position;
            backLegsSR.sortingLayerName = worldSortingLayer;

            coll.enabled = true;
            isPicked = false;

            anim.SetTrigger(anim_putTrigger);
        }
        #endregion

        #region Mining
        public virtual void Mine()
        {
            if(!currentOre.canGiveOre)
            {
                currentOre = null;
                oreTransform = null;

                MiningEnded();

                return;
            }

            int oreAmount = damage;
            currentOre.Take(damage);

            if(currentOre.amount < 0)
            {
                oreAmount += currentOre.amount;

                if (oreAmount <= 0)
                {
                    return;
                }
            }

            amount += oreAmount;
            allExtractedOre += oreAmount;
        }
        public virtual void MiningEnded()
        {
            return;
        }
        #endregion

        #region Health
        [Button]
        private void TakeDamage1()
        {
            TakeDamage(1);
        }

        public virtual void TakeDamage(float value)
        {
            health -= value;

            if(health <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            MiningEnded();
        }
        #endregion

        public void MauseDown(MouseClickType mouseClickType)
        {
            if(mouseClickType == MouseClickType.Left)
            {
                if (Vector2.Distance(myTransform.position, player.transform.position) > pickOreFromInventoryMaxDistance)
                    return;

                PlayerInventory.playerInventory.GiveItem(item, amount);
                amount = 0;
            }
        }
    }
}
