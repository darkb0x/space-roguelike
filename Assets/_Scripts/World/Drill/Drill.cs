using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Drill
{
    using World.Generation.Ore;
    using Player;
    using Player.Inventory;
    using Player.Pick;
    using Enemy;

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
        protected DrillInventoryVisual inventoryVisual;

        protected Transform myTransform;
        protected Transform oreTransform;
        protected PlayerController player;
        protected PlayerInteractObject playerInteractObject;
        protected PlayerInventory playerInventory;
        protected bool playerInZone = false;
        protected bool isPicked = false;

        protected bool isInitialized;

        [Header("Variables")]
        [SerializeField] protected int MiningDamage;
        [SerializeField] protected float TimeBtwMining;
        protected float currentTimeBtwMining;
        [ReadOnly] public bool IsMining = false;
        [Space]
        [Tag, SerializeField] protected string PlayerTag = "Player";

        [Header("Inventory")]
        [ReadOnly] public InventoryItem CurrentItem;
        [ReadOnly] public int ItemAmount;
        [ReadOnly, SerializeField] protected int allExtractedOre = 0;
        [Space]
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
        [SerializeField] protected EnemyTarget EnemyTarget;

        // renderrer
        [Header("Animation")]
        [SerializeField] protected Animator Anim;
        [AnimatorParam("Anim"), SerializeField] protected string Anim_putTrigger;
        [AnimatorParam("Anim"), SerializeField] protected string Anim_miningBool;

        [Header("Back Legs Renderer")]
        public PickedObjectPreRenderrer PreRenderPlaceObject;
        [SerializeField] protected SpriteRenderer BackLegsSR;
        [SortingLayer, SerializeField] protected string WorldSortingLayer;

        public virtual void Start()
        {
            playerInventory = Singleton.Get<PlayerInventory>();

            inventoryVisual = GetComponent<DrillInventoryVisual>();

            currentTimeBtwMining = TimeBtwMining;
            CurrentHealth = MaxHealth;

            MainColl.enabled = false;
            OreDetectColl.enabled = true;
            PlayerDetectColl.enabled = false;

            inventoryVisual.EnableVisual(false);
            PreRenderPlaceObject.gameObject.SetActive(false);
        }

        public virtual void Initialize()
        {
            if (isInitialized)
                return;

            player = FindObjectOfType<PlayerController>();
            playerTransform = player.transform;
            oreDetectColl_transform = OreDetectColl.transform;
            myTransform = transform;

            myTransform.SetParent(null);

            isPicked = true;

            player.pickObjSystem.SetPickedGameobj(gameObject);
            EnemyTarget.Initialize(this, null);

            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized)
                return;

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
            CurrentItem = CurrentOre.Item;
            oreDetectColl_transform.position = myTransform.position;

            myTransform.position = oreTransform.position;
            BackLegsSR.sortingLayerName = WorldSortingLayer;

            MainColl.enabled = true;
            OreDetectColl.enabled = false;
            PlayerDetectColl.enabled = true;
            isPicked = false;

            Anim.SetTrigger(Anim_putTrigger);
            PreRenderPlaceObject.transform.SetParent(null);
            PreRenderPlaceObject.gameObject.SetActive(false);

            inventoryVisual.UpdateVisual(CurrentItem, ItemAmount);
        }
        #endregion

        #region Mining
        public virtual void PlayerTakeItems()
        {
            if (!isInitialized)
                return;
            if (CurrentItem == null | isPicked)
                return;

            playerInventory.AddItem(CurrentItem, ItemAmount);
            ItemAmount = 0;
            inventoryVisual.UpdateVisual(CurrentItem, ItemAmount);

            if (CurrentOre != null)
            {
                if(CurrentOre.Amount < 0)
                {
                    EnableVisual(false);
                }
            }
            else
            {
                EnableVisual(false);
            }
        }
        public virtual void Mine()
        {
            int oreAmount = CurrentOre.Take(MiningDamage);

            ItemAmount += oreAmount;
            allExtractedOre += oreAmount;
            inventoryVisual.UpdateVisual(CurrentItem, ItemAmount);

            if (CurrentOre.Amount <= 0)
            {
                CurrentOre = null;
                oreTransform = null;

                MiningEnded();
            }
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
            player.pickObjSystem.SetPickedGameobj(null);
        }

        void IDamagable.Damage(float dmg, Enemy.EnemyTarget enemyTarget)
        {
            TakeDamage(dmg);
        }

        void IDamagable.Die()
        {
            Singleton.Get<EnemySpawner>().RemoveTarget(EnemyTarget);
            Die();
        }
        #endregion

        #region Collision triggers
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (isPicked && OreDetectColl.enabled)
            {
                if (collision.TryGetComponent<Ore>(out Ore ore))
                {
                    if (!currentOresList.Contains(ore))
                        currentOresList.Add(ore);
                }
            }
            if (collision.tag == PlayerTag && PlayerDetectColl.enabled)
            {
                if(isInitialized)
                {
                    EnableVisual(true);
                }
            }
        }
        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == PlayerTag && PlayerDetectColl.enabled)
            {
                playerInZone = true;
            }
        }
        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            if(isPicked)
            {
                if (collision.TryGetComponent<Ore>(out Ore ore))
                {
                    //ore.DisSelect();

                    if (currentOresList.Contains(ore))
                        currentOresList.Remove(ore);
                }
            }

            if (collision.tag == PlayerTag)
            {
                playerInZone = false;

                EnableVisual(false);
            }
        }
        #endregion

        #region Ore
        private Ore GetNearestOre()
        {
            Ore nearestOre = null;

            if (currentOresList.Count <= 0 | currentOresList == null)
            {
                PreRenderPlaceObject.gameObject.SetActive(false);
                return null;
            }

            foreach (var item in currentOresList)
            {
                if (nearestOre == null)
                {
                    if (item.currentDrill != null)
                        continue;
                    if (item.Amount <= 0)
                        continue;

                    nearestOre = item;
                    continue;
                }

                if (Vector2.Distance(myTransform.position, item.transform.position) < Vector2.Distance(myTransform.position, nearestOre.transform.position))
                {
                    if (item.currentDrill)
                        continue;
                    if (item.Amount <= 0)
                        continue;

                    nearestOre = item;
                }
            }

            if(nearestOre)
            {
                PreRenderPlaceObject.gameObject.SetActive(true);

                oreTransform = nearestOre.transform;
                PreRenderPlaceObject.transform.position = nearestOre.transform.position;
            }   
            else
            {
                PreRenderPlaceObject.gameObject.SetActive(false);
            }

            return nearestOre;
        }
        #endregion

        #region Break
        public virtual void Break()
        {
            if(ItemAmount > 0)
            {
                PlayerTakeItems();
            }

            if(isPicked && isInitialized)
            {
                player.pickObjSystem.PutCurrentGameobj(false);
            }

            foreach (var item in DroppedItemsAfterBroke)
            {
                playerInventory.AddItem(item.item, item.amount);
            }

            Singleton.Get<EnemySpawner>().RemoveTarget(EnemyTarget);
            Destroy(PreRenderPlaceObject.gameObject);
            Destroy(gameObject);
        }
        #endregion

        #region Visual
        protected void EnableVisual(bool enabled)
        {
            if(enabled)
            {
                bool canEnable = true;

                if (isPicked)
                    canEnable = false;
                if (CurrentOre?.Amount < 0 && ItemAmount == 0)
                    canEnable = false;

                inventoryVisual.EnableVisual(canEnable);
            }
            else
            {
                inventoryVisual.EnableVisual(false);
            }
        }
        #endregion
    }
}
