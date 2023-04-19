using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Globalization;

namespace Game.Audio
{
    [System.Serializable]
    public class SessionEvent
    {
        public struct WaveDuration
        {
            public float StartTime;
            public float EndTime;
        }

        public SessionEventType EventType;
        [ShowIf("EventType", SessionEventType.StartMicroWave), MaxValue(100), AllowNesting] public int PercentFromScore;
        [ShowIf("EventType", SessionEventType.StartWave), Label("From"), SerializeField, AllowNesting] private string WaveTimeStart;
        [ShowIf("EventType", SessionEventType.StartWave), Label("To"), SerializeField, AllowNesting] private string WaveTimeEnd;
        [Space]
        [OnValueChanged("OnTimeVariablesChanged"), SerializeField, AllowNesting] private int TimeMinute;
        [OnValueChanged("OnTimeVariablesChanged"), SerializeField, MaxValue(60), AllowNesting] private float TimeSecond;
        [SerializeField, ReadOnly, AllowNesting] private string m_StartTime = "0:00,00";
        public float StartTime { 
            get
            {
                return (TimeMinute * 60) + TimeSecond;
            }
        }

        public WaveDuration WaveTime
        {
            get
            {
                string[] startTimeParts = WaveTimeStart.Split(':');
                string[] endTimeParts = WaveTimeEnd.Split(':');

                // Start
                int startMinute = int.Parse(startTimeParts[0]);
                float startSeconds = float.Parse(startTimeParts[1], CultureInfo.InvariantCulture);
                float startTime = startMinute * 60f + startSeconds;

                // End
                int endMinute = int.Parse(endTimeParts[0]);
                float endSeconds = float.Parse(endTimeParts[1], CultureInfo.InvariantCulture);
                float endTime = endMinute * 60f + endSeconds;

                return new WaveDuration() { StartTime = startTime, EndTime = endTime };
            }
        }

        public void UpdateTimeVariables()
        {
            string minute = TimeMinute.ToString();

            string seconds;
            if (TimeSecond < 10)
                seconds = "0" + TimeSecond;
            else
                seconds = TimeSecond.ToString();

            m_StartTime = $"{minute}:{seconds} ({StartTime})";

            if(TimeSecond >= 60)
            {
                TimeSecond = 0;
                TimeMinute++;
            }
        }
    }

    [CreateAssetMenu(fileName = "Session timings", menuName = "Game/Mission/new Session timings")]
    public class SessionTimingsSO : ScriptableObject
    {
        [field: SerializeField] public AudioClip Music { get; set; }
        [SerializeField, ReorderableList] public List<SessionEvent> EventsList = new List<SessionEvent>();

        private void OnEnable()
        {
            foreach (var item in EventsList)
            {
                item.UpdateTimeVariables();
            }
        }
    }
}
