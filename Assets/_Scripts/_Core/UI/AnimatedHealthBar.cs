using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

namespace Game
{
    using Session.Artefact;
    using Session;
    using UI.HUD;

    public class AnimatedHealthBar : HUDElement
    {
        public GameObject Visual;
        [SerializeField] private Animator VisualAnim;
        [SerializeField, AnimatorParam("VisualAnim"), Label("Anim: 'Show' bool")] private string Anim_showBool;
        [Space]
        [SerializeField] private Image FillImage;
        [SerializeField] private Image ImpactFillImage;
        [Space]
        [SerializeField] private bool ShowHealthInPercent = true;
        [SerializeField] private TextMeshProUGUI ProgressText;

        [Foldout("Impact variables"), Label("Wait time"), SerializeField] private float ImpactVisualWaitTime = 0.2f;
        [Foldout("Impact variables"), Label("Speed"), SerializeField] private float ImpactVisualSpeed = 0.2f;

        [Space]
        [SerializeField, Range(0, 1), OnValueChanged("OnDebugProgressChanged")] private float DebugProgress;

        public override HUDElementID ID => HUDElementID.AnimatedHealthBar;

        private ArtefactDrill _artefactDrill;
        private bool _barEnabled;

        #region Inspector Events
#if UNITY_EDITOR
        private void OnDebugProgressChanged()
        {
            if (FillImage == null | ImpactFillImage == null)
                return;

            UpdateHealthBar(DebugProgress, 1);
        }
#endif
        #endregion

        public override void Initialize()
        {
            _artefactDrill = ServiceLocator.GetService<SessionManager>().ArtefactDrill;

            FillImage.fillAmount = 1f;
            ImpactFillImage.fillAmount = 1f;

            base.Initialize();
        }
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();

            _artefactDrill.OnMiningStarted += Show;
            _artefactDrill.OnMiningEnded += Hide;
            _artefactDrill.OnMiningUpdate += UpdateHealthBar;

            _uiWindowService.OnWindowOpened += _ => CancelImpactVisual();
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();

            _artefactDrill.OnMiningStarted -= Show;
            _artefactDrill.OnMiningEnded -= Hide;
            _artefactDrill.OnMiningUpdate -= UpdateHealthBar;

            _uiWindowService.OnWindowOpened += _ => CancelImpactVisual();
        }
        private void OnEnable()
        {
            if (_barEnabled && _initialized)
                Show();
        }

        public override void Hide()
        {
            _barEnabled = false;

            VisualAnim.SetBool(Anim_showBool, false);
        }

        public override void Show()
        {
            _barEnabled = true;

            VisualAnim.SetBool(Anim_showBool, true);
        }

        public void UpdateHealthBar(float current, float max)
        {
            float value = current / max;

            FillImage.fillAmount = value;
            StartCoroutine(UpdateImpactVisual());

            if(ShowHealthInPercent)
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

        private void CancelImpactVisual()
        {
            StopAllCoroutines();
            ImpactFillImage.fillAmount = FillImage.fillAmount;
        }
    }
}
