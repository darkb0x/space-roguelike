using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.CraftSystem
{
    using Player;

    public class Workbanch : MonoBehaviour
    {
        bool playerInZone;
        CSManager craftSystem;
        PlayerController currentPlayer;

        [Header("Variables")]
        [SerializeField] private KeyCode openKey;
        [Space]
        [SerializeField] private Transform playerPos;

        private void Start()
        {
            craftSystem = FindObjectOfType<CSManager>();
        }

        private void Update()
        {
            if (playerInZone)
            {
                if (Input.GetKeyDown(openKey))
                {
                    currentPlayer.StartCrafting(playerPos.position);
                    craftSystem.OpenCraftMenu(this);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent<PlayerController>(out PlayerController player))
            {
                currentPlayer = player;
                playerInZone = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            playerInZone = false;
        }
    }
}
