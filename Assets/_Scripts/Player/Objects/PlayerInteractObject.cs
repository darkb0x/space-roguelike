using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using System;

namespace Game.Player
{
    public delegate void CollisionEnter(Collider2D coll);

    public class PlayerInteractObject : MonoBehaviour
    {
        [Header("Action")]
        [SerializeField] private UnityEvent action;
        [Header("Player interact rules")]
        [Tag, SerializeField] private string playerTag = "Player";

        [ReadOnly] public bool playerInZone = false;

        public CollisionEnter OnPlayerEnter;
        public CollisionEnter OnPlayerStay;
        public CollisionEnter OnPlayerExit;

        private void Start()
        {
            GameInput.InputActions.Player.Interact.performed += Interact;
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if (!playerInZone)
                return;
            if (UIPanelManager.Instance.SomethinkIsOpened())
                return;

            action.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(playerTag))
            {
                OnPlayerEnter?.Invoke(collision);
                playerInZone = true;
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag(playerTag))
            {
                OnPlayerStay?.Invoke(collision);
                playerInZone = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(playerTag))
            {
                OnPlayerExit?.Invoke(collision);
                playerInZone = false;
            }
        }

        private void OnDisable()
        {
            GameInput.InputActions.Player.Interact.performed -= Interact;
        }
    }
}