using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Session
{
    using SaveData;

    public class SessionTimingsVisual : MonoBehaviour
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

        private SessionManager SessionManager;
        private UISettingsData currentUISettingsData => SaveDataManager.Instance.CurrentUISettingsData;

        private List<MarkData> markList;

        private void Start()
        {
            if (!VidgetButton.interactable)
            {
                Enable(false);
                return;
            }
            Enable(currentUISettingsData.EnableTimeline);

            SessionManager = Singleton.Get<SessionManager>();

            m_Slider.minValue = 0;
            m_Slider.maxValue = SessionManager.SessionTimings.Music.length;

            SessionManager.OnEventReached += EventReached;

            InitializeMarks();
        }

        private void Update()
        {
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

        public void Enable(bool enabled)
        {
            gameObject.SetActive(enabled);

            currentUISettingsData.EnableTimeline = enabled;
            currentUISettingsData.Save();
        }
        public void Enable()
        {
            Enable(!gameObject.activeSelf);
        }
    }
}
