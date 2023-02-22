using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game.Player
{
    public class PlayerInteractObject : MonoBehaviour
    {
        [Header("Action")]
        [SerializeField] private UnityEvent action;
        [Header("Player interact rules")]
        [Tag, SerializeField] private string playerTag = "Player";

        [HideInInspector] public bool playerInZone = false;

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
                playerInZone = true;
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(playerTag))
                playerInZone = false;
        }
    }
}
