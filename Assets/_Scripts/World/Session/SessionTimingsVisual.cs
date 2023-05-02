using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Session
{
    public class SessionTimingsVisual : MonoBehaviour
    {
        private struct MarkData
        {
            public Image Visual;
            public float Time;

            private bool enabled
            {
                get
                {
                    return Visual.color.a == 1;
                }
                set
                {
                    float disabledAlpha = 0.7f;
                    float enabledAlpha = 1f;

                    Visual.color = new Color(Visual.color.r, Visual.color.g, Visual.color.b, value ? enabledAlpha : disabledAlpha);
                }
            }

            public void Enabled(bool value)
            {
                enabled = value;
            }
        }

        [SerializeField] private GameObject MarkPrefab;

        private SessionManager SessionManager;

        private List<MarkData> markList;
        private Slider slider;

        private void Start()
        {
            slider = GetComponent<Slider>();
            SessionManager = Singleton.Get<SessionManager>();

            slider.minValue = 0;
            slider.maxValue = SessionManager.SessionTimings.Music.length;

            SessionManager.OnEventReached += EventReached;

            InitializeMarks();
        }

        private void Update()
        {
            slider.value = Mathf.Clamp(SessionManager.currentTime, slider.minValue, slider.maxValue);
        }

        private void InitializeMarks()
        {
            markList = new List<MarkData>();

            float minValue = slider.minValue;
            float maxValue = slider.maxValue;
            float sliderWidth = slider.GetComponent<RectTransform>().rect.width;

            foreach (var timing in SessionManager.SessionTimings.EventsList)
            {
                RectTransform markRect = Instantiate(MarkPrefab, transform).GetComponent<RectTransform>();

                float normalizedValue = (timing.StartTime - minValue) / (maxValue - minValue);
                float markPosX = (sliderWidth * normalizedValue) - (sliderWidth / 2f) + markRect.rect.width / 2;

                markRect.anchoredPosition = new Vector2(markPosX, 0);

                MarkData mark = new MarkData() { Visual = markRect.GetComponent<Image>(), Time = timing.StartTime };
                mark.Enabled(true);
                markList.Add(mark);
            }
        }

        private void EventReached(SessionEvent sessionEvent)
        {
            foreach (var mark in markList)
            {
                if(mark.Time <= sessionEvent.StartTime)
                {
                    mark.Enabled(false);
                }
                else
                {
                    mark.Enabled(true);
                }
            }
        }
    }
}
