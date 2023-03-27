using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace Game.Player.Visual
{
    public class PlayerVisual : MonoBehaviour
    {
        [Header("Oxygen")]
        [SerializeField] private Image OxygenBarUI;
        [SerializeField] private GameObject OxygenBarVisual;

        [Header("Health")]
        [SerializeField] private Sprite FullHeartSprite;
        [SerializeField] private Sprite DamagedHeartSprite;
        [SerializeField] private Transform HeartsTransform;

        [Header("Animator")]
        public Animator Anim;
        [Space]
        [AnimatorParam("Anim"), SerializeField] string Anim_horizontal;
        [AnimatorParam("Anim"), SerializeField] string Anim_vertical;
        [AnimatorParam("Anim"), SerializeField] string Anim_isRunning;
        [AnimatorParam("Anim"), SerializeField] string Anim_isCrafting;

        private List<Image> HeartsImages = new List<Image>();
        private bool updateOxygenVisual = true;
        private Vector2 heartImageSize = new Vector2(50, 50);
        private Vector2 shadowEffectDistance = new Vector2(5, -5);

        public void InitializeHealthVisual(int maxHealth)
        {
            ClearHeartsVisual();

            for (int i = 0; i < maxHealth; i++)
            {
                GameObject gameObject = new GameObject();
                gameObject.name = $"Heart ({i})";

                Image heartImage = gameObject.AddComponent<Image>();
                heartImage.sprite = FullHeartSprite;
                heartImage.rectTransform.sizeDelta = heartImageSize;

                Shadow heartShadow = gameObject.AddComponent<Shadow>();
                heartShadow.effectDistance = shadowEffectDistance;

                HeartsImages.Add(heartImage);

                gameObject.transform.SetParent(HeartsTransform);
                gameObject.transform.localScale = Vector3.one;
            }
        }
        private void ClearHeartsVisual()
        {
            if (HeartsTransform.childCount == 0)
                return;

            int childCount = HeartsTransform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(HeartsTransform.GetChild(i).gameObject);
            }
        }

        public void PlayerLookDirection(Vector2 direction)
        {
            Anim.SetFloat(Anim_horizontal, direction.x);
            Anim.SetFloat(Anim_vertical, direction.y);
        }
        public void PlayerIdleAnimation()
        {
            Anim.SetBool(Anim_isRunning, false);
        }
        public void PlayerRunAnimation()
        {
            Anim.SetBool(Anim_isRunning, true);
        }

        public void EnableOxygenVisual(bool enabled)
        {
            updateOxygenVisual = enabled;

            OxygenBarVisual.SetActive(updateOxygenVisual);
        }
        public void UpdateOxygenVisual(float current, float max)
        {
            if (!updateOxygenVisual)
                return;

            OxygenBarUI.fillAmount = current / max;
        }
        public void UpdateHealthVisual(int currentHealth)
        {
            for (int i = 0; i < HeartsImages.Count; i++)
            {
                if (i < currentHealth)
                {
                    HeartsImages[i].sprite = FullHeartSprite;
                }
                else
                {
                    HeartsImages[i].sprite = DamagedHeartSprite;
                }
            }
        }
    }
}
