using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Player
{
    using Drone;
    using Inventory;
    using Pick;
    using Visual;
    using SaveData;

    public class PlayerController : MonoBehaviour, IDamagable
    {
        [Header("Oxygen")]
        [SerializeField] private float oxygen = 50;
        [SerializeField] private float maxOxygen = 100f;
        [SerializeField] private float oxygenUseSpeed = 1.3f;
        [SerializeField] private bool DoOxygenCycle = true;

        [Header("Health")]
        [SerializeField] private float health = 10;
        [SerializeField] private float maxHealth;
        [SerializeField] private bool DoHealthCycle = true;

        [Header("Movement")]
        [SerializeField] private float speed;

        [Header("Visual")]
        [SerializeField] private PlayerVisual Visual;

        [Header("components")]
        public PlayerPickObjects pickObjSystem;
        [HideInInspector] public PlayerInventory inventory;
        [SerializeField] private Collider2D MainColl;
        [SerializeField] private Enemy.EnemyTarget EnemyTarget;

        Vector2 moveInput;

        Rigidbody2D rb;
        Camera cam;
        new Transform transform;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool canLookAround = true;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            transform = GetComponent<Transform>();

            inventory = FindObjectOfType<PlayerInventory>();
            cam = Camera.main;
            EnemyTarget.Initialize(this);

            health = maxHealth;
            Visual.InitializeHealthVisual((int)health);

            oxygen = maxOxygen;
            Visual.EnableOxygenVisual(DoOxygenCycle);
        }

        private void Update()
        {
            moveInput = GameInput.Instance.GetMoveInput();

            //animation
            if(moveInput.magnitude > 0)
            {
                if (!UIPanelManager.Instance.SomethinkIsOpened())
                {
                    if(canLookAround)
                    {
                        Visual.PlayerLookDirection(moveInput);
                    }
                }
            }
            else
            {
                if (!UIPanelManager.Instance.SomethinkIsOpened())
                {
                    Vector3 mousePos = cam.ScreenToWorldPoint(GameInput.Instance.GetMousePosition());
                    Vector2 dir = -(transform.position - mousePos).normalized;

                    if (canLookAround)
                    {
                        Visual.PlayerLookDirection(dir);
                    }
                }
            }

            if (canMove)
            {
                if (moveInput.magnitude > 0)
                {
                    Visual.PlayerRunAnimation();
                }
                else
                {
                    Visual.PlayerIdleAnimation();
                }
            }
            else
                Visual.PlayerIdleAnimation();

            //oxygen
            if(DoOxygenCycle)
                OxygenCycle();
        }

        private void FixedUpdate()
        {
            if(canMove)
                rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
        }

        #region Oxygen
        private void OxygenCycle()
        {
            if(oxygen <= 0)
            {
                Die();
            }

            oxygen -= (Time.deltaTime * oxygenUseSpeed);
            Visual.UpdateOxygenVisual(oxygen, maxOxygen);
        }

        public void AddOxygen(float amount)
        {
            oxygen = Mathf.Clamp(oxygen + amount, 0, maxOxygen);
        }
        #endregion

        #region Health
        [Button]
        public void TakeDamage()
        {
            TakeDamage(1);
        }

        public void TakeDamage(float value)
        {
            health -= Mathf.RoundToInt(value);

            Visual.UpdateHealthVisual((int)health);

            if (health <= 0)
            {
                Die();
            }
        }
        private void Die()
        {
            GameData.Instance.ResetSessionData();
        }

        void IDamagable.Damage(float dmg, Enemy.EnemyTarget enemyTarget)
        {
            if (!DoHealthCycle)
                return;

            TakeDamage(dmg);
        }
        void IDamagable.Die()
        {
            if (!DoHealthCycle)
                return;

            Enemy.EnemySpawner.Instance.RemoveTarget(EnemyTarget);
            Die();
        }
        #endregion
        
        #region Drone
        /*
        private void AddDrone(object sender, EventArgs e)
        {
            Debug.Log("Drone used");
            if (selectedDrone)
            {
                if (!selectedDrone.isPicked) selectedDrone.Init();
            }
        }
        */
        #endregion

        #region Collision triggers
        DroneAI selectedDrone;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<DroneAI>(out DroneAI drone))
            {
                if (!drone.isPicked) selectedDrone = drone;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            selectedDrone = null;
        }
        #endregion

        #region Animation Control
        public void StopPlayerMove(Transform posTransform)
        {
            canMove = false;
            transform.position = posTransform.position;
        }
        public void StopPlayerMove()
        {
            canMove = false;
        }
        public void ContinuePlayerMove()
        {
            canMove = true;
        }

        public void LockPlayerPosition(Transform posPosition)
        {
            canMove = false;
            transform.SetParent(posPosition);
            transform.localPosition = Vector2.zero;
            MainColl.enabled = false;
        }
        public void UnlockPlayerPosition()
        {
            transform.SetParent(null);
            canMove = true;
            MainColl.enabled = true;
        }

        public void LockPlayerLook(bool enabled)
        {
            canLookAround = !enabled;
        }
        #endregion
    }
}