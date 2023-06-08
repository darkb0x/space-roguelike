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
        [Space]
        [SerializeField] private GameObject Canvas;
        [SerializeField] private UnityEngine.UI.Image OxygenAmountVisual;

        public bool isUsed { get; private set; }
        private float oxygenAmount;
        private PlayerController currentPlayer;

        private void Start()
        {
            oxygenAmount = m_OxygenAmount;

            isUsed = false;
            BallonVisual.sprite = BallonNormal;

            OxygenAmountVisual.fillAmount = oxygenAmount / m_OxygenAmount;
            Canvas.SetActive(false);
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

            OxygenAmountVisual.fillAmount = oxygenAmount / m_OxygenAmount;

            if (oxygenAmount <= 0)
            {
                BallonVisual.sprite = BallonUsed;
                isUsed = true;
                Canvas.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (isUsed)
                return;

            if(coll.TryGetComponent(out PlayerController player))
            {
                currentPlayer = player;
                Canvas.SetActive(true);
            }
        }
        private void OnTriggerExit2D(Collider2D coll)
        {
            if (isUsed)
                return;

            if (coll.TryGetComponent(out PlayerController player))
            {
                currentPlayer = null;
                Canvas.SetActive(false);
            }
        }
    }
}
