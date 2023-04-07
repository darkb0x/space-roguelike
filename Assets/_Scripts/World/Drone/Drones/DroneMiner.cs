using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Drone
{
    using World.Generation.Ore;
    using Player.Inventory;

    public class DroneMiner : DroneAI
    {
        [Header("DroneMiner")]
        [SerializeField] private InventoryItem CurrentItem;
        [SerializeField] private int MaxItemAmount;
        [Space]
        [SerializeField] private float m_TimeBtwMiningHits = 1f;
        [SerializeField] private int ItemsPerHit = 1;
        [Space]
        [SerializeField] private Collider2D OreDetectionColl;
        [Space]
        [SerializeField, ReadOnly] private bool InOrbit;
        [SerializeField] private float MaxDistanceBtwPlayerAndDrone = 4f;
        
        [Header("Visual")]
        [SerializeField] private Vector2 OffsetOnore = new Vector2(0, 0.5f);
        [Space]
        [SerializeField] private GameObject LaserVisual;
        [Space]
        [SerializeField] private SpriteRenderer NewItemIcon;
        [Space]
        [SerializeField] private Animator Anim;
        [SerializeField, AnimatorParam("Anim")] private string Anim_newItemTrigger;

        private List<Ore> oreInVision = new List<Ore>();
        private Ore currentOre;
        private Transform playerTransform;
        private Transform oreDetectionOreTransform;
        private Vector2 targetPos;
        private float timeBtwMiningHits;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, MaxDistanceBtwPlayerAndDrone);
        }

        private void Start()
        {
            oreDetectionOreTransform = OreDetectionColl.transform;

            LaserVisual.SetActive(false);
            NewItemIcon.color = new Color(1, 1, 1, 0);

            timeBtwMiningHits = m_TimeBtwMiningHits;
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                InOrbit = true;
                return;
            }

            oreDetectionOreTransform.position = playerTransform.position;
            LaserVisual.SetActive(currentOre != null);

            if (Vector2.Distance(playerTransform.position, transform.position) > MaxDistanceBtwPlayerAndDrone)
            {
                InOrbit = true;
                currentOre = null;
            }
            else
            {
                if (currentOre == null)
                {
                    currentOre = GetFreeOre();
                }
                else
                {
                    Mine();
                }
            }
        }

        private void Mine()
        {
            if(timeBtwMiningHits <= 0)
            {
                if (currentOre.Amount <= 0)
                {
                    currentOre = null;
                    InOrbit = true;
                    return;
                }
                if (currentOre.currentDrill != null)
                {
                    currentOre = null;
                    InOrbit = true;
                    return;
                }

                if(currentOre.Amount >= ItemsPerHit)
                {
                    currentOre.Take(ItemsPerHit);
                    PlayerInventory.Instance.AddItem(CurrentItem, ItemsPerHit, false);
                }
                else
                {
                    currentOre.Take(currentOre.Amount);
                    PlayerInventory.Instance.AddItem(CurrentItem, currentOre.Amount, false);
                }

                Anim.SetTrigger(Anim_newItemTrigger);

                targetPos = (Vector2)currentOre.transform.position + OffsetOnore;
                InOrbit = false;

                timeBtwMiningHits = m_TimeBtwMiningHits;
            }
            else
            {
                timeBtwMiningHits -= Time.deltaTime;
            }
        }

        public override void Initialize(PlayerDronesController pdc)
        {
            base.Initialize(pdc);

            playerTransform = pdc.transform;
            InOrbit = true;
        }

        public override void RotationUpdate(Transform point, float direction, float rangeFromPoint)
        {
            if(InOrbit)
            {
                targetPos = (Vector2)point.position + new Vector2(Mathf.Sin(direction * Mathf.Deg2Rad), Mathf.Cos(direction * Mathf.Deg2Rad)) * rangeFromPoint;
            }

            transform.position = Vector2.Lerp(transform.position, targetPos, 3 * Time.deltaTime);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);

            if(collision.TryGetComponent<Ore>(out Ore ore))
            {
                oreInVision.Add(ore);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Ore>(out Ore ore))
            {
                if(ore.currentDrone == this)
                {
                    ore.currentDrone = null;
                }

                if(oreInVision.Contains(ore))
                {
                    oreInVision.Remove(ore);
                }
            }
        }

        #region Utilities
        private Ore GetFreeOre()
        {
            foreach (var ore in oreInVision)
            {
                if (ore.currentDrill != null)
                    continue;
                if (ore.currentDrone != null)
                    continue;
                if (ore.Amount <= 0)
                    continue;

                ore.currentDrone = this;

                CurrentItem = ore.Item;
                NewItemIcon.sprite = CurrentItem.LowSizeIcon;

                return ore;
            }
            return null;
        }
        #endregion
    }
}
