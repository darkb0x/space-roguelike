using System.Collections;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Player
{
    using Inventory;
    using Pick;
    using Visual;
    using SaveData;

    public class PlayerController : MonoBehaviour, IDamagable, IMovableTarget
    {
        [Header("Oxygen")]
        [SerializeField] private float oxygen = 50;
        [SerializeField] private float LowOxygenValue;
        [SerializeField] private float maxOxygen = 100f;
        [SerializeField] private float oxygenUseSpeed = 1.3f;
        public bool DoOxygenCycle = true;

        [Header("Health")]
        [SerializeField] private float health = 10;
        [SerializeField] private float maxHealth;
        public bool DoHealthCycle = true;
        [Space]
        [SerializeField] private float InvulnerabilityTime = 0.2f;

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
        public bool invulnerability = false;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool isDied = false;
        [HideInInspector] public bool canLookAround = true;

        private UIPanelManager UIPanelManager;

        private void Start()
        {
            UIPanelManager = Singleton.Get<UIPanelManager>();

            rb = GetComponent<Rigidbody2D>();
            myTransform = transform;

            inventory = FindObjectOfType<PlayerInventory>();
            cam = Camera.main;
            EnemyTarget.Initialize(this, this);

            health = maxHealth;
            Visual.InitializeHealthVisual();

            if(!DoOxygenCycle)
            {
                oxygen = maxOxygen;
                Visual.UpdateOxygenVisual(oxygen, maxOxygen);
            }
        }

        private void Update()
        {
            moveInput = GameInput.Instance.GetMoveInput();

            //animation
            if(moveInput.magnitude > 0)
            {
                if (!UIPanelManager.SomethinkIsOpened())
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
                if (!UIPanelManager.SomethinkIsOpened())
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
        public Vector3 GetMoveDirection()
        {
            if(canMove)
            {
                return moveInput;
            } 
            else
            {
                return Vector3.zero;
            }
        }

        #region Oxygen
        private void OxygenCycle()
        {
            if(oxygen <= 0)
            {
                Die();
            }

            if(oxygen < LowOxygenValue)
            {
                Visual.PlayerLowOxygen(true, oxygen, LowOxygenValue);
            }
            else
            {
                Visual.PlayerLowOxygen(false, oxygen, LowOxygenValue);
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
            health = Mathf.Clamp(health - Mathf.RoundToInt(value), 0, maxHealth);

            Visual.UpdateHealthVisual((int)health);

            LogUtility.WriteLog($"Player got damage, {health}/{maxHealth}");

            if (health <= 0)
            {
                Die();
            }

            StartCoroutine(SetInvulnerability());
        }
        public void RegenerateHealth()
        {
            health = maxHealth;

            Visual.UpdateHealthVisual((int)health);
        }

        private void Die()
        {
            if (isDied)
                return;

            StopPlayerMove();
            MainColl.enabled = false;
            DoOxygenCycle = false;

            SaveDataManager.Instance.CurrentSessionData.Reset();
            UIPanelManager.CloseAllPanel();

            Visual.PlayerDead();

            isDied = true;

            LogUtility.WriteLog("Player died");
        }

        private IEnumerator SetInvulnerability()
        {
            invulnerability = true;
            StartCoroutine(Visual.PlayerHurt(InvulnerabilityTime));
            yield return new WaitForSeconds(InvulnerabilityTime);
            invulnerability = false;
        }

        void IDamagable.Damage(float dmg, Enemy.EnemyTarget enemyTarget)
        {
            if (!DoHealthCycle)
                return;
            if (invulnerability)
                return;

            TakeDamage(dmg);
        }
        void IDamagable.Die()
        {
            if (!DoHealthCycle)
                return;
            if (invulnerability)
                return;

            Singleton.Get<Enemy.EnemySpawner>().RemoveTarget(EnemyTarget);
            Die();
        }
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