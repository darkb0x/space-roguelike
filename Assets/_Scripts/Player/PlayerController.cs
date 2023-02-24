using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Game.Player
{
    using Drone;
    using Inventory;
    using CraftSystem;

    public class PlayerController : MonoBehaviour, IDamagable
    {
        [Header("Oxygen")]
        [SerializeField, ProgressBar("Oxygen", "maxOxygen", EColor.Blue)] private float oxygen = 50;
        [SerializeField] private float maxOxygen = 100f;
        [SerializeField] private float oxygenUseSpeed = 1.3f;
        [SerializeField] private bool DoOxygenCycle = true;
        [Space]
        [SerializeField] private Image oxygenBarUI;

        [Header("Health")]
        [SerializeField, ProgressBar("Health", "maxHealth", EColor.Red)] private float health = 10;
        [SerializeField] private float maxHealth;
        [SerializeField] private bool DoHealthCycle = true;
        [Space]
        [SerializeField] private Image healthBarUI;

        [Header("movement")]
        [SerializeField] private float speed;

        [Header("animator")]
        public Animator anim;
        [Space]
        [AnimatorParam("anim"), SerializeField] string anim_horizontal;
        [AnimatorParam("anim"), SerializeField] string anim_vertical;
        [AnimatorParam("anim"), SerializeField] string anim_isRunning;
        [AnimatorParam("anim"), SerializeField] string anim_isCrafting;

        [Header("components")]
        public PlayerPickObjects pickObjSystem;
        [HideInInspector] public PlayerInventory inventory;
        [SerializeField] private Enemy.EnemyTarget EnemyTarget;

        Vector2 moveInput;

        Rigidbody2D rb;
        Camera cam;
        new Transform transform;
        [HideInInspector] public bool canMove = true;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            transform = GetComponent<Transform>();

            inventory = FindObjectOfType<PlayerInventory>();
            cam = Camera.main;
            EnemyTarget.Initialize(this);

            health = maxHealth;
            if (healthBarUI != null)
            {
                healthBarUI.fillAmount = health / maxHealth;
            }
            else
            {
                Debug.LogError("healthBarUI is null");
                DoHealthCycle = false;
            }

            oxygen = maxOxygen;
            if (oxygenBarUI != null)
            {
                oxygenBarUI.fillAmount = oxygen / maxOxygen;
            }
            else
            {
                Debug.LogError("oxygenBarUI in null");
                DoOxygenCycle = false;
            }
        }

        private void Update()
        {
            moveInput = GameInput.Instance.GetMoveInput();

            //animation
            if(moveInput.magnitude > 0)
            {
                if (!UIPanelManager.Instance.SomethinkIsOpened())
                {
                    anim.SetFloat(anim_horizontal, moveInput.x);
                    anim.SetFloat(anim_vertical, moveInput.y);
                }
            }
            else
            {
                if (!UIPanelManager.Instance.SomethinkIsOpened())
                {

                    Vector3 mousePos = cam.ScreenToWorldPoint(GameInput.Instance.GetMousePosition());
                    Vector2 dir = -(transform.position - mousePos).normalized;

                    anim.SetFloat(anim_horizontal, dir.x);
                    anim.SetFloat(anim_vertical, dir.y);
                }
            }

            if(canMove)
                anim.SetBool(anim_isRunning, moveInput.magnitude > 0);
            else
                anim.SetBool(anim_isRunning, false);

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
            oxygenBarUI.fillAmount = oxygen / maxOxygen;
        }

        public void AddOxygen(float amount)
        {
            oxygen = Mathf.Clamp(oxygen + amount, 0, maxOxygen);
        }
        #endregion

        #region Health
        public void TakeDamage(float value)
        {
            health -= value;
            healthBarUI.fillAmount = health / maxHealth;

            if (health <= 0)
            {
                Die();
            }
        }
        private void Die()
        {
            LoadCraftUtility.Instance.ClearUnlockedCrafts();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

            EnemySpawner.instance.RemoveTarget(EnemyTarget);
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
        public void StopPlayerMove(Vector3 pos)
        {
            canMove = false;
        }
        public void ContinuePlayerMove()
        {
            canMove = true;
        }
        #endregion
    }

}