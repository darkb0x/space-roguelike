using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Audio
{
    [System.Serializable]
    public class SessionEvent
    {
        public SessionEventType EventType;
        [ShowIf("EventType", SessionEventType.StartMicroWave), MaxValue(100), AllowNesting] public int PercentFromScore;
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
