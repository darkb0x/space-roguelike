using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Player
{
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

        private Vector2 moveInput;
        private Vector2 lookDir;

        private Rigidbody2D rb;
        private Camera cam;
        private Transform myTransform;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool isDied = false;
        [HideInInspector] public bool canLookAround = true;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            myTransform = transform;

            inventory = FindObjectOfType<PlayerInventory>();
            cam = Camera.main;
            EnemyTarget.Initialize(this);

            health = maxHealth;
            Visual.InitializeHealthVisual((int)health);

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
                        lookDir = moveInput;
                        Visual.PlayerLookDirection(moveInput);
                    }
                }
            }
            else
            {
                if (!UIPanelManager.Instance.SomethinkIsOpened())
                {
                    Vector3 mousePos = cam.ScreenToWorldPoint(GameInput.Instance.GetMousePosition());
                    lookDir = -(myTransform.position - mousePos).normalized;

                    if (canLookAround)
                    {
                        Visual.PlayerLookDirection(lookDir);
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
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void TakeDamage()
        {
            TakeDamage(1);
        }

        public void TakeDamage(float value)
        {
            if (!DoHealthCycle)
                return;

            health -= Mathf.RoundToInt(value);

            Visual.UpdateHealthVisual((int)health);

            if (health <= 0)
            {
                Die();
            }
        }
        private void Die()
        {
            if (!DoHealthCycle)
                return;
            if (isDied)
                return;

            StopPlayerMove();
            MainColl.enabled = false;
            DoOxygenCycle = false;

            GameData.Instance.ResetSessionData();
            UIPanelManager.Instance.CloseAllPanel();

            Visual.PlayerDead(lookDir.x);

            isDied = true;
        }

        void IDamagable.Damage(float dmg, Enemy.EnemyTarget enemyTarget)
        {
            TakeDamage(dmg);
        }
        void IDamagable.Die()
        {
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

        #region Animation Control
        public void StopPlayerMove(Transform posTransform)
        {
            canMove = false;
            myTransform.position = posTransform.position;
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
            myTransform.SetParent(posPosition);
            myTransform.localPosition = Vector2.zero;
            MainColl.enabled = false;
        }
        public void UnlockPlayerPosition()
        {
            myTransform.SetParent(null);
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