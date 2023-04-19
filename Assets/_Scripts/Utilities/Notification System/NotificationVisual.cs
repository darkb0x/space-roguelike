using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Utilities.Notifications.Visual
{
    public class NotificationVisual : MonoBehaviour
    {
        [SerializeField] private Image ItemIconImage;
        [SerializeField] private Transform ItemHighlight_Visual;
        [Space]
        [SerializeField] private TextMeshProUGUI ItemTitleText;
        [Space]
        [SerializeField] private CanvasGroup CanvasGroup;
        [Space]
        [SerializeField] private Transform VisualTransform;

        private bool showHighlight = false;

        public void Initialize(Sprite icon, string title, bool isHighlighting, float destroyTime)
        {
            ItemIconImage.sprite = icon;
            ItemTitleText.text = title;

            showHighlight = isHighlighting;
            ItemHighlight_Visual.gameObject.SetActive(isHighlighting);

            StartCoroutine(ShowNotification());

            Invoke("StartHidingNotification", destroyTime);
        }

        private void Update()
        {
            if (showHighlight)
            {
                float rotationSpeed = -30f;
                ItemHighlight_Visual.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);
            }
        }

        private void StartHidingNotification()
        {
            StartCoroutine(HideNotification());
        }
        private IEnumerator HideNotification()
        {
            float speed = 3f;
            while (CanvasGroup.alpha > 0)
            {
                CanvasGroup.alpha = Mathf.MoveTowards(CanvasGroup.alpha, 0, speed * Time.deltaTime);
                yield return null;
            }
            Destroy(gameObject);
        }

        private IEnumerator ShowNotification()
        {
            float timeForMove = 2f;
            float showSpeed = 2f / timeForMove;
            float moveSpeed = 5f / timeForMove;
            float time = Time.time + timeForMove;

            CanvasGroup.alpha = 0;
            VisualTransform.localPosition = new Vector3(0, -130f);

            while (Time.time < time)
            {
                VisualTransform.localPosition = Vector3.Lerp(VisualTransform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
                CanvasGroup.alpha = Mathf.MoveTowards(CanvasGroup.alpha, 1, showSpeed * Time.deltaTime);

                yield return null;
            }
            CanvasGroup.alpha = 1;
        }
    }
}
