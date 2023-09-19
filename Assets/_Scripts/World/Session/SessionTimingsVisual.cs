using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Session
{
    using Save;
    using UI.HUD;

    public class SessionTimingsVisual : HUDElement
    {
        private struct MarkData
        {
            public Image Visual;
            public float Time;

            public void SetEnabled(bool value)
            {
                float disabledAlpha = 0.7f;
                float enabledAlpha = 1f;

                Visual.color = new Color(Visual.color.r, Visual.color.g, Visual.color.b, value ? enabledAlpha : disabledAlpha);
            }
        }

        [SerializeField] private Slider m_Slider;
        [SerializeField] private GameObject MarkPrefab;
        [Space]
        [SerializeField] private Button VidgetButton;
        [SerializeField] private Image VidgetVisual;

        public override HUDElementID ID => HUDElementID.SessionWaveTimings;

        private SessionManager SessionManager;
        private UISaveData currentUISettingsData => SaveManager.UISaveData;

        private List<MarkData> markList;

        public override void Initialize()
        {
            if (currentUISettingsData.EnableNotifications)
                Show();
            else
                Hide();

            SessionManager = ServiceLocator.GetService<SessionManager>();

            m_Slider.minValue = 0;
            m_Slider.maxValue = SessionManager.SessionTimings.Music.length;

            InitializeMarks();

            base.Initialize();
        }

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            SessionManager.OnEventReached += EventReached;
        }
        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            SessionManager.OnEventReached -= EventReached;
        }

        private void Update()
        {
            if(View.gameObject.activeSelf)
                m_Slider.value = Mathf.Clamp(SessionManager.currentTime, m_Slider.minValue, m_Slider.maxValue);
        }

        private void InitializeMarks()
        {
            markList = new List<MarkData>();

            float minValue = m_Slider.minValue;
            float maxValue = m_Slider.maxValue;
            float sliderWidth = m_Slider.GetComponent<RectTransform>().rect.width;

            foreach (var timing in SessionManager.SessionTimings.EventsList)
            {
                RectTransform markRect = Instantiate(MarkPrefab, transform).GetComponent<RectTransform>();

                float normalizedValue = (timing.StartTime - minValue) / (maxValue - minValue);
                float markPosX = (sliderWidth * normalizedValue) - (sliderWidth / 2f) + markRect.rect.width / 2;

                markRect.anchoredPosition = new Vector2(markPosX, 0);

                MarkData mark = new MarkData() { Visual = markRect.GetComponent<Image>(), Time = timing.StartTime };
                mark.SetEnabled(true);
                markList.Add(mark);
            }
        }

        private void EventReached(SessionEvent sessionEvent)
        {
            foreach (var mark in markList)
            {
                if(mark.Time <= sessionEvent.StartTime)
                {
                    mark.SetEnabled(false);
                }
                else
                {
                    mark.SetEnabled(true);
                }
            }
        }

        public override void Show()
        {
            base.Show();
            currentUISettingsData.EnableNotifications = true;
            currentUISettingsData.Save();
        }

        public override void Hide()
        {
            base.Hide();
            currentUISettingsData.EnableNotifications = false;
            currentUISettingsData.Save();
        }

        public void ChangeEnabled()
        {
            currentUISettingsData.EnableNotifications = !currentUISettingsData.EnableNotifications;
            if (currentUISettingsData.EnableNotifications)
                Show();
            else
                Hide();
        }
    }
}
