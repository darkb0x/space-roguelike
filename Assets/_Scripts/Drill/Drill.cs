using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

namespace Game.Drill
{
    using World;
    using Player;
    using Player.Inventory;

    public abstract class Drill : MonoBehaviour, IDamagable
    {
        [System.Serializable]
        public struct dropped_item
        {
            public InventoryItem item;
            public int amount;
        }

        private List<Ore> currentOresList = new List<Ore>();
        private Transform oreDetectColl_transform;
        private Transform playerTransform;

        protected Transform myTransform;
        protected bool isPicked = true;
        protected Transform oreTransform;
        protected PlayerController player;
        protected bool playerInZone = false;
        protected PlayerInteractObject playerInteractObject;

        [Header("Variables")]
        [SerializeField] protected int MiningDamage;
        [SerializeField] protected float TimeBtwMining;
        protected float currentTimeBtwMining;
        [ReadOnly] public bool IsMining = false;
        [Space]
        [Tag, SerializeField] protected string PlayerTag = "Player";

        [Header("Inventory")]
        public InventoryItem Item;
        public int Amount;
        [ReadOnly, SerializeField] protected int allExtractedOre = 0;
        [Space]
        [ReadOnly] public Ore CurrentOre;

        [Header("Colliders")]
        [SerializeField] protected Collider2D MainColl;
        [SerializeField] protected Collider2D OreDetectColl;
        [SerializeField] protected Collider2D PlayerDetectColl;

        [Header("Break System")]
        public List<dropped_item> DroppedItemsAfterBroke = new List<dropped_item>();

        [Header("Health")]
        public float MaxHealth = 10;
        [ReadOnly] public float CurrentHealth;
        [SerializeField] private Enemy.EnemyTarget EnemyTarget;

        // renderrer
        [Header("Animation")]
        [SerializeField] protected Animator Anim;
        [AnimatorParam("Anim"), SerializeField] protected string Anim_putTrigger;
        [AnimatorParam("Anim"), SerializeField] protected string Anim_miningBool;

        [Header("Back Legs Renderer")]
        [SerializeField] protected SpriteRenderer BackLegsSR;
        [SortingLayer, SerializeField] protected string WorldSortingLayer;

        public virtual void Start()
        {
            player = FindObjectOfType<PlayerController>();
            myTransform = transform;
            oreDetectColl_transform = OreDetectColl.transform;
            playerTransform = player.transform;
            EnemyTarget.Initialize(this);

            currentTimeBtwMining = TimeBtwMining;
            CurrentHealth = MaxHealth;

            Initialize();
        }

        public virtual void Initialize()
        {
            isPicked = true;

            MainColl.enabled = false;
            OreDetectColl.enabled = true;
            PlayerDetectColl.enabled = false;

            player.pickObjSystem.SetPickedGameobj(gameObject);
        }

        private void Update()
        {
            if (isPicked)
            {
                oreDetectColl_transform.position = playerTransform.position;

                return;
            }

            // mining animation & mining
            if(CurrentOre && IsMining)
            {
                Anim.SetBool(Anim_miningBool, true);

                if(currentTimeBtwMining <= 0)
                {
                    Mine();

                    currentTimeBtwMining = TimeBtwMining;
                }
                else
                {
                    currentTimeBtwMining -= Time.deltaTime;
                }
            }
            else
            {
                Anim.SetBool(Anim_miningBool, false);
            }
        }
        private void FixedUpdate()
        {
            if(isPicked)
            {
                CurrentOre = GetNearestOre();
            }
        }

        #region Put
        public bool CanPut()
        {
            return CurrentOre;
        }
        public virtual void Put()
        {
            CurrentOre.currentDrill = this;
            Item = CurrentOre.item;
            oreDetectColl_transform.position = myTransform.position;

            myTransform.position = oreTransform.position;
            BackLegsSR.sortingLayerName = WorldSortingLayer;

            MainColl.enabled = true;
            OreDetectColl.enabled = false;
            PlayerDetectColl.enabled = true;
            isPicked = false;

            Anim.SetTrigger(Anim_putTrigger);

            DisSelectAllOres();
        }
        #endregion

        #region Mining
        public virtual void PlayerTakeItems()
        {
            PlayerInventory.instance.GiveItem(Item, Amount);
            Amount = 0;
        }
        public virtual void Mine()
        {
            if(!CurrentOre.canGiveOre)
            {
                CurrentOre = null;
                oreTransform = null;

                MiningEnded();

                return;
            }

            int oreAmount = MiningDamage;
            CurrentOre.Take(MiningDamage);

            if(CurrentOre.amount < 0)
            {
                oreAmount += CurrentOre.amount;

                if (oreAmount <= 0)
                {
                    return;
                }
            }

            Amount += oreAmount;
            allExtractedOre += oreAmount;
        }
        public virtual void MiningEnded()
        {
            return;
        }
        #endregion

        #region Health

        public virtual void TakeDamage(float value)
        {
            CurrentHealth -= value;

            if(CurrentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            MiningEnded();
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

        #region Collision triggers
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isPicked && OreDetectColl.enabled)
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
            if (collision.tag == PlayerTag && PlayerDetectColl.enabled)
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

            if (collision.tag == PlayerTag)
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
                    if (item.currentDrill)
                        continue;
                    if (!item.canGiveOre)
                        continue;

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
            if(nearestOre)
            {
                nearestOre.Select();

                oreTransform = nearestOre.transform;
            }   

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

        #region Break
        public virtual void Break()
        {
            if(Amount > 0)
            {
                PlayerTakeItems();
            }

            if(isPicked)
            {
                player.pickObjSystem.PutCurrentGameobj(false);
            }

            foreach (var item in DroppedItemsAfterBroke)
            {
                PlayerInventory.instance.GiveItem(item.item, item.amount);
            }

            EnemySpawner.instance.RemoveTarget(EnemyTarget);
            Destroy(gameObject);
        }
        #endregion
    }
}
