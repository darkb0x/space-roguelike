using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.SceneLoading
{
    public class LoadingScreenVisual : MonoBehaviour
    {
        [SerializeField] private Sprite[] LoadingIconFrames;
        [SerializeField] private Image LoadingIcon;
        [Space]
        [SerializeField] private TextMeshProUGUI LoadingProgress;
        [Space]
        [SerializeField] private CanvasGroup CanvasGroup;

        public void PlayAnimation()
        {
            CanvasGroup.alpha = 1;
            StartCoroutine(AnimateLoadingIcon());
            UpdateProgress(0);
        }

        public void UpdateProgress(float progress)
        {
            LoadingProgress.text = (progress / 0.9f * 100f).ToString("F0") + "%";
        }

        float frameTime = 0.08f;
        int currentFrame;
        private IEnumerator AnimateLoadingIcon()
        {
            while(true)
            {
                currentFrame++;
                if (currentFrame >= LoadingIconFrames.Length)
                {
                    currentFrame = 0;
                }

                LoadingIcon.sprite = LoadingIconFrames[currentFrame];

                yield return new WaitForSecondsRealtime(frameTime);
            }
        }

        private void StopPlayingLoadingIconAnimation()
        {
            StopCoroutine(AnimateLoadingIcon());
        }
    }
}
