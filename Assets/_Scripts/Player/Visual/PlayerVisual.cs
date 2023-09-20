using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using NaughtyAttributes;

namespace Game.Player
{
    using SceneLoading;

    public class PlayerVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer SpriteVisual;

        [Header("Health")]
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

        private Vignette vignettePostProcessing;

        private float lowOxygenSpeed = 0.3f;
        private float lowOxygenTargetIdensity;

        public void Initialize()
        {
            //DeathPanel.SetActive(false);

            var camera = ServiceLocator.GetService<CameraController>();
            if(camera.MainVolume.profile.TryGet(out Vignette vignette))
            {
                vignettePostProcessing = vignette;
            }
        }

        private void Update()
        {
            vignettePostProcessing.intensity.Override(Mathf.MoveTowards(vignettePostProcessing.intensity.value, lowOxygenTargetIdensity, lowOxygenSpeed * Time.deltaTime));
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

        public void GoMenu()
        {
            LoadSceneUtility.LoadSceneAsyncVisualize(ManuSceneID);
        }
    }
}
