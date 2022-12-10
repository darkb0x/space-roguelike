using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Player
{
    using Drone;
    using Inventory;
    using Turret;

    public class PlayerController : MonoBehaviour
    {
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
        bool canMove = true;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            transform = GetComponent<Transform>();

            inventory = FindObjectOfType<PlayerInventory>();
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

            //animation
            anim.SetFloat(anim_runHorizontal, moveInput.x);
            anim.SetFloat(anim_runVertical, moveInput.y);
            if(canMove)
                anim.SetBool(anim_isRunning, moveInput.magnitude > 0);
        }

        private void FixedUpdate()
        {
            if(canMove)
                rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
        }

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
            anim.SetBool(anim_isRunning, false);
        }
        #endregion
    }

}