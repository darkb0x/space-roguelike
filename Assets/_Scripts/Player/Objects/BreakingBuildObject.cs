using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class BreakingBuildObject : MonoBehaviour
    {
        [SerializeField, NaughtyAttributes.Tag] private string PlayerTag = "Player";
        [Space]
        [SerializeField] private GameObject BreakProgressGameObj;
        [SerializeField] private Image BreakProgressImage;
        [SerializeField] protected float BreakTime = 5;
        [Space]
        [SerializeField] private UnityEvent Break;

        private float currentBreakProgress;
        private bool playerInZone = false;
        private bool canBeBreak = true;

        private void Start()
        {
            GameInput.InputActions.Player.Break.canceled += EndBreaking;

            currentBreakProgress = BreakTime;

            BreakProgressGameObj.SetActive(false);
        }

        public void DisableBreaking()
        {
            canBeBreak = false;

            if (currentBreakProgress > 0)
            {
                StartCoroutine(EndBreaking());
            }
        }

        private void Update()
        {
            if (!canBeBreak)
                return;

            if (GameInput.InputActions.Player.Break.IsPressed())
                Breaking();
        }

        private void Breaking()
        {
            if (!playerInZone)
                return;

            BreakProgressGameObj.SetActive(true);
            if (currentBreakProgress <= 0)
            {
                GameInput.InputActions.Player.Break.canceled -= EndBreaking;
                Break.Invoke();
            }
            else
            {
                currentBreakProgress -= Time.deltaTime;
                BreakProgressImage.fillAmount = Mathf.Abs((currentBreakProgress / BreakTime) - 1);
            }
        }
        private void EndBreaking(InputAction.CallbackContext context)
        {
            StartCoroutine(EndBreaking());
        }
        private IEnumerator EndBreaking()
        {
            while (BreakProgressImage.fillAmount >= 0.01f)
            {
                currentBreakProgress = Mathf.Lerp(currentBreakProgress, BreakTime, 0.2f); ;

                BreakProgressImage.fillAmount = Mathf.Abs((currentBreakProgress / BreakTime) - 1);

                yield return null;
            }

            currentBreakProgress = BreakTime;
            BreakProgressGameObj.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag(PlayerTag))
                playerInZone = true;
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(PlayerTag))
                playerInZone = false;
        }

        private void OnDisable()
        {
            GameInput.InputActions.Player.Break.canceled -= EndBreaking;
        }
    }
}
