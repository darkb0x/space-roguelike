using UnityEngine;

namespace Game.World
{
    using Player;
    using Input;

    public class OxygenBallon : MonoBehaviour
    {
        [SerializeField] private float m_OxygenAmount;
        [SerializeField] private float UseSpeed = 3f;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer BallonVisual;
        [SerializeField] private Material OutlineMaterial;
        [SerializeField] private Sprite BallonNormal;
        [SerializeField] private Sprite BallonUsed;
        [Space]
        [SerializeField] private GameObject Canvas;
        [SerializeField] private UnityEngine.UI.Image OxygenAmountVisual;

        private PlayerController currentPlayer;
        private Material defaultMaterial;
        public bool isUsed { get; private set; }
        private float oxygenAmount;

        private void Start()
        {
            oxygenAmount = m_OxygenAmount;
            defaultMaterial = BallonVisual.material;
            BallonVisual.sprite = BallonNormal;
            isUsed = false;

            OxygenAmountVisual.fillAmount = oxygenAmount / m_OxygenAmount;
            Canvas.SetActive(false);
        }

        private void Update()
        {
            if (isUsed)
                return;
            if (currentPlayer == null)
                return;
            
            if(InputManager.PlayerInputHandler.InteractEvent.IsPressed())
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
                Canvas.SetActive(false);
                BallonVisual.material = defaultMaterial;

                isUsed = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (isUsed)
                return;

            if(coll.TryGetComponent(out currentPlayer))
            {
                Canvas.SetActive(true);
                BallonVisual.material = OutlineMaterial;
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
                BallonVisual.material = defaultMaterial;
            }
        }
    }
}
