using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Player
{
    public class PlayerInteractObject : MonoBehaviour
    {
        [Header("Action")]
        [SerializeField] private UnityEvent action;
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        [Header("Player interact rules")]
        [NaughtyAttributes.Tag, SerializeField] private string playerTag = "Player";

        bool playerInZone = false;

        private void Update()
        {
            if (!playerInZone)
                return;
            if (UIPanelManager.manager.SomethinkIsOpened())
                return;

            if(Input.GetKeyDown(interactKey))
            {
                action.Invoke();
            }
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
