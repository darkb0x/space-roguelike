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
        List<Ore> currentOresList = new List<Ore>();

        [HideInInspector] public Transform myTransform;
        [HideInInspector] public bool isPicked = true;
        [HideInInspector] public Transform oreTransform;
        [HideInInspector] public PlayerController player;
        [HideInInspector] public bool playerInZone = false;

        [Header("Variables")]
        public int damage;
        public float timeBtwMining;
        [HideInInspector] public float currentTimeBtwMining;
        [ReadOnly] public bool isMining = false;
        [Space]
        [ReadOnly] public float health;
        public float maxHealth;
        [Space]
        [Tag] public string playerTag = "Player";

        [Header("Inventory")]
        public InventoryItem item;
        public int amount;
        [ReadOnly] public int allExtractedOre = 0;
        [Space]
        [ReadOnly] public Ore currentOre;

        [Header("Colliders")]
        public Collider2D mainColl;
        public Collider2D oreDetectColl;
        public Collider2D playerDetectColl;
        
        [Header("Animation")]
        public Animator anim;
        [AnimatorParam("anim")] public string anim_putTrigger;
        [AnimatorParam("anim")] public string anim_miningBool;
        
        [Header("Back Legs Renderer")]
        public SpriteRenderer backLegsSR;
        [SortingLayer] public string worldSortingLayer;

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

            mainColl.enabled = false;
            oreDetectColl.enabled = true;
            playerDetectColl.enabled = false;

            player.pickObjSystem.SetPickedGameobj(gameObject);
        }

        private void Update()
        {
            if (isPicked)
                return;

            // mining animation & mining
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

            // player take items
            if(playerInZone)
            {
                if(Input.GetKeyDown(KeyCode.E))
                {
                    PlayerInventory.playerInventory.GiveItem(item, amount);
                    amount = 0;
                }
            }
        }
        private void FixedUpdate()
        {
            if(isPicked)
            {
                currentOre = GetNearestOre();
            }
        }

        #region Put
        public bool CanPut()
        {
            return currentOre;
        }
        public virtual void Put()
        {
            currentOre.currentDrill = this;
            item = currentOre.item;

            myTransform.position = oreTransform.position;
            backLegsSR.sortingLayerName = worldSortingLayer;

            mainColl.enabled = true;
            oreDetectColl.enabled = false;
            playerDetectColl.enabled = true;
            isPicked = false;

            anim.SetTrigger(anim_putTrigger);

            DisSelectAllOres();
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

        #region Collision triggers
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isPicked && oreDetectColl.enabled)
            {
                if (collision.TryGetComponent<Ore>(out Ore ore))
                {
                    if (!currentOresList.Contains(ore))
                        currentOresList.Add(ore);
                }
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == playerTag && playerDetectColl.enabled)
            {
                playerInZone = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if(isPicked)
            {
                if (collision.TryGetComponent<Ore>(out Ore ore))
                {
                    ore.DisSelect();

                    if (currentOresList.Contains(ore))
                        currentOresList.Remove(ore);
                }
            }

            if (collision.tag == playerTag)
            {
                playerInZone = false;
            }
        }
        #endregion

        #region Ore
        private Ore GetNearestOre()
        {
            Ore nearestOre = null;

            if (currentOresList.Count <= 0 | currentOresList == null)
            {
                return null;
            }

            foreach (var item in currentOresList)
            {
                if (nearestOre == null)
                {
                    nearestOre = item;
                    continue;
                }

                if (Vector2.Distance(myTransform.position, item.transform.position) < Vector2.Distance(myTransform.position, nearestOre.transform.position))
                {
                    if (item.currentDrill)
                        continue;
                    if (!item.canGiveOre)
                        continue;

                    nearestOre = item;
                }
            }

            DisSelectAllOres();
            nearestOre.Select();

            oreTransform = nearestOre.transform;     

            return nearestOre;
        }
        private void DisSelectAllOres()
        {
            foreach (var item in currentOresList)
            {
                item.DisSelect();
            }
        }
        #endregion
    }
}