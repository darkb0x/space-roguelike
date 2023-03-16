using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    using Player;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class OxygenBallon : MonoBehaviour
    {
        [SerializeField] private float OxygenAmount;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer BallonVisual;
        [SerializeField] private Sprite BallonNormal;
        [SerializeField] private Sprite BallonUsed;

        public bool isUsed { get; private set; }
        private PlayerInteractObject interactObject;
        private PlayerController currentPlayer;

        private void Start()
        {
            interactObject = GetComponent<PlayerInteractObject>();

            isUsed = false;
            BallonVisual.sprite = BallonNormal;

            interactObject.OnPlayerEnter += OnPlayerEnter;
            interactObject.OnPlayerExit += OnPlayerExit;
        }

        public void UseOxygenBallon()
        {
            if (isUsed)
                return;
            if (currentPlayer == null)
                currentPlayer = FindObjectOfType<PlayerController>();

            currentPlayer.AddOxygen(OxygenAmount);
            BallonVisual.sprite = BallonUsed;
            isUsed = true;
        }

        private void OnPlayerEnter(Collider2D coll)
        {
            currentPlayer = coll.GetComponent<PlayerController>();
        }
        private void OnPlayerExit(Collider2D coll)
        {
            currentPlayer = null;
        }

        private void OnDisable()
        {
            interactObject.OnPlayerEnter -= OnPlayerEnter;
            interactObject.OnPlayerExit -= OnPlayerExit;
        }
    }
}
