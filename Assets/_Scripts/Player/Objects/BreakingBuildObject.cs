using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class BreakingBuildObject : MonoBehaviour
    {
        [Header("Break Visual")]
        [SerializeField] private GameObject BreakProgressGameObj;
        [SerializeField] private Image BreakProgressImage;
        [SerializeField] private float BreakTime = 2.5f;

        [Header("Break Interaction")]
        [SerializeField, Tooltip("Radius in which player have to stay for break build.")] private float Radius = 1.3f;
        [SerializeField] private LayerMask PlayerLayer;

        [Space]
        [SerializeField] private UnityEvent Break;

        private Coroutine endBreakingCoroutine;
        private float currentBreakProgress;
        private bool playerInZone = false;
        private bool canBeBreak = true;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        private void Start()
        {
            GameInput.InputActions.Player.Break.performed += StartBreaking;
            GameInput.InputActions.Player.Break.canceled += EndBreaking;

            currentBreakProgress = BreakTime;

            BreakProgressGameObj.SetActive(false);
        }

        public void DisableBreaking()
        {
            canBeBreak = false;

            if (currentBreakProgress > 0)
            {
                EndBreaking();
            }
        }

        private void Update()
        {
            if (!canBeBreak)
                return;

            if (GameInput.InputActions.Player.Break.IsPressed())
                Breaking();
        }

        private void StartBreaking(InputAction.CallbackContext obj)
        {
            playerInZone = Physics2D.OverlapCircleAll(transform.position, Radius, PlayerLayer).Length > 0;
        }

        private void Breaking()
        {
            if (!playerInZone)
            {
                if(endBreakingCoroutine == null)
                {
                    EndBreaking();
                }
                return;
            }

            playerInZone = Physics2D.OverlapCircleAll(transform.position, Radius, PlayerLayer).Length > 0;

            BreakProgressGameObj.SetActive(true);
            if (currentBreakProgress <= 0)
            {
                GameInput.InputActions.Player.Break.canceled -= EndBreaking;
                Break?.Invoke();
            }
            else
            {
                currentBreakProgress -= Time.deltaTime;
                BreakProgressImage.fillAmount = Mathf.Abs((currentBreakProgress / BreakTime) - 1);
            }
        }
        private void EndBreaking()
        {
            endBreakingCoroutine = StartCoroutine(EndBreakingCoroutine());
        }
        private void EndBreaking(InputAction.CallbackContext context)
        {
            EndBreaking();
        }
        private IEnumerator EndBreakingCoroutine()
        {
            while (BreakProgressImage.fillAmount >= 0.01f)
            {
                currentBreakProgress = Mathf.Lerp(currentBreakProgress, BreakTime, 0.2f); ;

                BreakProgressImage.fillAmount = Mathf.Abs((currentBreakProgress / BreakTime) - 1);

                yield return null;
            }

            currentBreakProgress = BreakTime;
            BreakProgressGameObj.SetActive(false);
            endBreakingCoroutine = null;
        }

        private void OnDisable()
        {
            GameInput.InputActions.Player.Break.performed -= StartBreaking;
            GameInput.InputActions.Player.Break.canceled -= EndBreaking;
        }
    }
}
