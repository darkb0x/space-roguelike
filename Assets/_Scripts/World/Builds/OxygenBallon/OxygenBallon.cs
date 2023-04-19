using UnityEngine;

namespace Game.World
{
    using Player;

    public class OxygenBallon : MonoBehaviour
    {
        [SerializeField] private float m_OxygenAmount;
        [SerializeField] private float UseSpeed = 3f;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer BallonVisual;
        [SerializeField] private Sprite BallonNormal;
        [SerializeField] private Sprite BallonUsed;

        public bool isUsed { get; private set; }
        private float oxygenAmount;
        private PlayerInteractObject interactObject;
        private PlayerController currentPlayer;

        private void Start()
        {
            interactObject = GetComponent<PlayerInteractObject>();

            oxygenAmount = m_OxygenAmount;

            isUsed = false;
            BallonVisual.sprite = BallonNormal;
        }

        private void Update()
        {
            if (isUsed)
                return;
            if (currentPlayer == null)
                return;
            
            if(GameInput.InputActions.Player.Interact.IsPressed())
            {
                UseOxygenBallon();
            }
        }

        public void UseOxygenBallon()
        {
            float value = Time.deltaTime * UseSpeed;

            currentPlayer.AddOxygen(value);
            oxygenAmount -= value;

            if(oxygenAmount <= 0)
            {
                isUsed = true;

                BallonVisual.sprite = BallonUsed;
                isUsed = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (isUsed)
                return;

            if(coll.TryGetComponent<PlayerController>(out PlayerController player))
            {
                currentPlayer = player;
            }
        }
        private void OnTriggerExit2D(Collider2D coll)
        {
            if (isUsed)
                return;

            if (coll.TryGetComponent<PlayerController>(out PlayerController player))
            {
                currentPlayer = null;
            }
        }
    }
}
