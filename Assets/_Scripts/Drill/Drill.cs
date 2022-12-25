using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Drill
{
    using World;
    using Player;
    using Player.Inventory;

    public abstract class Drill : MonoBehaviour
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
        [Space]
        public float oreFindingRadius = 1.2f;
        public LayerMask oreLayer;

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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, oreFindingRadius);
        }

        public virtual void Start()
        {
            player = FindObjectOfType<PlayerController>();
            myTransform = transform;

            currentTimeBtwMining = timeBtwMining;

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

            coll.enabled = true;
            isPicked = false;

            anim.SetTrigger(anim_putTrigger);
        }

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

                if(oreAmount <= 0)
                {
                    return;
                }
            }

            amount += oreAmount;
        }
        public virtual void MiningEnded()
        {
            return;
        }
    }
}
