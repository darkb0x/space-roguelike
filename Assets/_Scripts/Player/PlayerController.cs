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

    public class PlayerController : MonoBehaviour
    {
        [Header("Oxygen")]
        [SerializeField, ProgressBar("Oxygen", "maxOxygen", EColor.Blue)] private float oxygen = 50;
        [SerializeField] private float maxOxygen = 100f;
        [SerializeField] private float oxygenUseSpeed = 1.3f;
        [Space]
        [SerializeField] private Image oxygenBarUI;

        [Header("Health")]
        [SerializeField, ProgressBar("Health", "maxHealth", EColor.Red)] private float health = 10;
        [SerializeField] private float maxHealth;
        [Space]
        [SerializeField] private Image healthBarUI;

        [Header("movement")]
        [SerializeField] private float speed;

        [Header("animator")]
        public Animator anim;
        [Space]
        [AnimatorParam("anim"), SerializeField] string anim_runHorizontal;
        [AnimatorParam("anim"), SerializeField] string anim_runVertical;
        [AnimatorParam("anim"), SerializeField] string anim_isRunning;
        [AnimatorParam("anim"), SerializeField] string anim_isCrafting;

        [Header("components")]
        public PlayerPickObjects pickObjSystem;
        [HideInInspector] public PlayerInventory inventory;

        Vector2 moveInput;

        Rigidbody2D rb;
        new Transform transform;
        [HideInInspector] public bool canMove = true;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            transform = GetComponent<Transform>();

            inventory = FindObjectOfType<PlayerInventory>();

            health = maxHealth;
            oxygen = maxOxygen;

            oxygenBarUI.fillAmount = oxygen / maxOxygen;
            healthBarUI.fillAmount = health / maxHealth;
        }

        private void Update()
        {
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (selectedDrone)
                {
                    if (!selectedDrone.isPicked) selectedDrone.Init();
                }
            }
            if(Input.GetKeyDown(KeyCode.R))
            {
                LoadCraftUtility.loadCraftUtility.ClearUnlockedCrafts();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            //animation
            anim.SetFloat(anim_runHorizontal, moveInput.x);
            anim.SetFloat(anim_runVertical, moveInput.y);
            if(canMove)
                anim.SetBool(anim_isRunning, moveInput.magnitude > 0);
            else
                anim.SetBool(anim_isRunning, false);

            //oxygen
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
            Debug.Log("Player is die");
            return;
        }
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
        public void StartCrafting(Vector3 pos)
        {
            canMove = false;
        }
        public void EndCrafting()
        {
            canMove = true;
        }
        #endregion
    }

}