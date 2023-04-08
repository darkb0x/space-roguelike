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
        [SerializeField, ReadOnly] private bool InOrbit;
        [SerializeField] private float MaxDistanceBtwPlayerAndDrone = 4f;
        
        [Header("Visual")]
        [SerializeField] private Vector2 OffsetOnOre = new Vector2(0, 0.6f);
        [Space]
        [SerializeField] private GameObject LaserVisual;
        [Space]
        [SerializeField] private SpriteRenderer NewItemIcon;
        [Space]
        [SerializeField] private Animator Anim;
        [SerializeField, AnimatorParam("Anim")] private string Anim_newItemTrigger;

        public Ore currentOre;
        private Transform playerTransform;
        private Vector2 targetPos;
        private float timeBtwMiningHits;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, MaxDistanceBtwPlayerAndDrone);
        }

        private void Start()
        {
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


            if (Vector2.Distance(playerTransform.position, transform.position) <= MaxDistanceBtwPlayerAndDrone)
            {
                if(currentOre != null)
                {
                    if(Vector2.Distance(currentOre.transform.position, transform.position) <= 1f)
                    {
                        LaserVisual.SetActive(true);
                    }
                    else
                    {
                        LaserVisual.SetActive(false);
                    }

                    InOrbit = false;
                    Mine();
                }
                else
                {
                    LaserVisual.SetActive(false);

                    SetFree();
                }
            }
            else
            {
                LaserVisual.SetActive(false);

                SetFree();
            }
        }

        private void Mine()
        {
            if(timeBtwMiningHits <= 0)
            {
                if (currentOre.Amount <= 0)
                {
                    SetFree();
                    return;
                }
                if (currentOre.currentDrill != null)
                {
                    SetFree();
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

                targetPos = (Vector2)currentOre.transform.position + OffsetOnOre;
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

            transform.position = Vector2.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }

        public bool SetOre(Ore ore)
        {
            if (Vector2.Distance(ore.transform.position, playerTransform.position) > MaxDistanceBtwPlayerAndDrone)
                return false;

            if (ore.currentDrill != null)
                return false;
            if (ore.currentDrone != null)
                return false;
            if (ore.Amount <= 0)
                return false;

            currentOre = ore;
            currentOre.currentDrone = this;

            CurrentItem = ore.Item;
            NewItemIcon.sprite = CurrentItem.LowSizeIcon;

            return true;
        }

        public void SetFree()
        {
            if (currentOre != null)
            {
                currentOre.currentDrone = null;
                currentOre = null;
            }

            InOrbit = true;
        }
    }
}
