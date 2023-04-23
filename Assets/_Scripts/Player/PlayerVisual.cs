using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using NaughtyAttributes;

namespace Game.Player.Visual
{
    using Utilities.LoadScene;

    public class PlayerVisual : MonoBehaviour
    {
        [SerializeField] private Volume MainVolume;

        [Space]
        [SerializeField] private PlayerController Player;
        [SerializeField] private SpriteRenderer SpriteVisual;

        [Header("Oxygen")]
        [SerializeField] private Image OxygenBarUI;
        [SerializeField] private GameObject OxygenBarVisual;

        [Header("Health")]
        [SerializeField] private Sprite FullHeartSprite;
        [SerializeField] private Sprite DamagedHeartSprite;
        [SerializeField] private Transform HeartsTransform;
        [Space]
        [SerializeField] private GameObject DeathPanel;
        [SerializeField, Scene] private int ManuSceneID;

        [Header("Animator")]
        public Animator Anim;
        [Space]
        [AnimatorParam("Anim"), SerializeField] string Anim_horizontalFloat;
        [AnimatorParam("Anim"), SerializeField] string Anim_verticalFloat;
        [AnimatorParam("Anim"), SerializeField] string Anim_isRunningBool;
        [AnimatorParam("Anim"), SerializeField] string Anim_isPickingSmthBool;
        [AnimatorParam("Anim"), SerializeField] string Anim_deadTrigger;

        private List<Image> HeartsImages = new List<Image>();
        private bool updateOxygenVisual = true;
        private Vector2 heartImageSize = new Vector2(50, 50);
        private Vector2 shadowEffectDistance = new Vector2(5, -5);

        private Vignette vignettePostProcessing;

        private float lowOxygenSpeed = 0.3f;
        private float lowOxygenTargetIdensity;

        private void Start()
        {
            DeathPanel.SetActive(false);

            if(MainVolume.profile.TryGet<Vignette>(out Vignette vignette))
            {
                vignettePostProcessing = vignette;
            }
        }

        private void Update()
        {
            vignettePostProcessing.intensity.Override(Mathf.MoveTowards(vignettePostProcessing.intensity.value, lowOxygenTargetIdensity, lowOxygenSpeed * Time.deltaTime));
        }

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
            Anim.SetFloat(Anim_horizontalFloat, direction.x);
            Anim.SetFloat(Anim_verticalFloat, direction.y);
        }
        public void PlayerIdleAnimation()
        {
            Anim.SetBool(Anim_isRunningBool, false);
        }
        public void PlayerRunAnimation()
        {
            Anim.SetBool(Anim_isRunningBool, true);
        }
        public void PlayerPick(bool enabled)
        {
            Anim.SetBool(Anim_isPickingSmthBool, enabled);
        }
        public IEnumerator PlayerHurt(float time)
        {
            float minAlpha = 0.2f;
            float targetAlpha = minAlpha;
            float speed = 4.5f;
            float endTime = Time.time + time;

            while(Time.time < endTime)
            {
                if (SpriteVisual.color.a == 1)
                    targetAlpha = minAlpha;
                if (SpriteVisual.color.a == minAlpha)
                    targetAlpha = 1f;

                while(SpriteVisual.color.a != targetAlpha)
                {
                    Color targetColor = new Color(1, 1, 1, Mathf.MoveTowards(SpriteVisual.color.a, targetAlpha, speed * Time.deltaTime));

                    SpriteVisual.color = targetColor;
                    yield return null;
                }
            }
            targetAlpha = 1f;

            while (SpriteVisual.color.a != targetAlpha)
            {
                Color targetColor = new Color(1, 1, 1, Mathf.MoveTowards(SpriteVisual.color.a, targetAlpha, speed * Time.deltaTime));

                SpriteVisual.color = targetColor;
                yield return null;
            }
        }
        public void PlayerDead()
        {
            float direction = Anim.GetFloat(Anim_horizontalFloat);

            if (direction < 0)
            {
                SpriteVisual.flipX = true;
            }
            else
            {
                SpriteVisual.flipX = false;
            }
            SpriteVisual.sortingOrder = 5;
            Anim.SetTrigger(Anim_deadTrigger);

            DeathPanel.SetActive(true);
        }
        public void PlayerLowOxygen(bool isLow, float currentOxygen, float lowValue)
        {
            if(!isLow)
            {
                lowOxygenTargetIdensity = 0;
            }
            else
            {
                lowOxygenTargetIdensity = Mathf.InverseLerp(lowValue, 0, currentOxygen);
            }
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

        public void GoMenu()
        {
            LoadSceneUtility.LoadSceneAsyncVisualize(ManuSceneID);
        }
    }
}
