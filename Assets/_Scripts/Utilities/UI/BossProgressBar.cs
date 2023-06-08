using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

namespace Game
{
    public class BossProgressBar : MonoBehaviour
    {
        public GameObject Visual;
        [SerializeField] private Animator VisualAnim;
        [SerializeField, AnimatorParam("VisualAnim"), Label("Anim: 'Show' bool")] private string Anim_showBool;
        [Space]
        [SerializeField] private Image FillImage;
        [SerializeField] private Image ImpactFillImage;
        [Space]
        [SerializeField, OnValueChanged("OnColorInInspectorChanged")] private Color FillColor = Color.white;
        [SerializeField, OnValueChanged("OnColorInInspectorChanged")] private Color ImpactFillColor = Color.white;
        [Space]
        [SerializeField] private bool ShowProgressInPercent = true;
        [SerializeField] private TextMeshProUGUI ProgressText;

        [Foldout("Impact variables"), Label("Wait time"), SerializeField] private float ImpactVisualWaitTime = 0.2f;
        [Foldout("Impact variables"), Label("Speed"), SerializeField] private float ImpactVisualSpeed = 0.2f;

        [Space]
        [SerializeField, Range(0, 1), OnValueChanged("OnDebugProgressChanged")] private float DebugProgress;

        private bool barEnabled;
        #region Inspector Events
        private void OnColorInInspectorChanged()
        {
            if(FillImage != null)
            {
                FillImage.color = FillColor;
            } 
            if(ImpactFillImage != null)
            {
                ImpactFillImage.color = ImpactFillColor;
            }
        }
        private void OnDebugProgressChanged()
        {
            if (FillImage == null | ImpactFillImage == null)
                return;

            UpdateProgressBar(DebugProgress, 1);
        }
        #endregion

        public void EnableProgressBar(bool enabled)
        {
            if(!Visual.activeSelf)
            {
                Visual.SetActive(true);
            }
            barEnabled = enabled;

            VisualAnim.SetBool(Anim_showBool, enabled);
        }

        public void UpdateProgressBar(float current, float max)
        {
            float value = current / max;

            FillImage.fillAmount = value;
            StartCoroutine(UpdateImpactVisual());

            if(ShowProgressInPercent)
            {
                int valueInPecent = Mathf.RoundToInt(value * 100);
                ProgressText.text = valueInPecent + "%";
            }
            else
            {
                ProgressText.text = $"{current}/{max}";
            }
        }

        private IEnumerator UpdateImpactVisual()
        {
            yield return new WaitForSeconds(ImpactVisualWaitTime);

            while(FillImage.fillAmount != ImpactFillImage.fillAmount)
            {
                float value = ImpactFillImage.fillAmount;
                value = Mathf.MoveTowards(value, FillImage.fillAmount, ImpactVisualSpeed * Time.deltaTime);

                ImpactFillImage.fillAmount = value;

                yield return null;
            }

            ImpactFillImage.fillAmount = FillImage.fillAmount;
        }

        private void OnEnable()
        {
            if(barEnabled)
                EnableProgressBar(barEnabled);
        }
    }
}
