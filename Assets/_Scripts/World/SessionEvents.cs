using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Game
{
    using Audio;
    using Enemy;

    public enum SessionEventType
    {
        StartMicroWave,
        StartWave
    }

    public class SessionEvents : MonoBehaviour
    {
        public static SessionEvents Instance;

        [Header("Session Theme")]
        [SerializeField, NaughtyAttributes.Expandable] private SessionTimingsSO SessionTimings;

        [Header("Audio")]
        [SerializeField] private AudioSource MusicAudioSource;

        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI DebugText;

        private int eventsCount = 0;
        private int currentEvent = 0;
        [SerializeField] private float currentTime;

        private void Awake()
        {
            Instance = this;

            eventsCount = SessionTimings.EventsList.Count;
            currentEvent = 0;
        }

        private void Start()
        {
            Pause.Instance.OnGamePaused += OnGamePaused;

            MusicAudioSource.clip = SessionTimings.Music;
            MusicAudioSource.Play();
        }

        private void Update()
        {
            DebugText.text = $"Time: {currentTime}\n" +
                             $"Event: {currentEvent}";

            if (currentEvent >= eventsCount)
                return;

            currentTime += Time.deltaTime;

            if (currentTime >= SessionTimings.EventsList[currentEvent].StartTime)
            {
                ActivateEvent(SessionTimings.EventsList[currentEvent]);

                if (currentEvent < eventsCount)
                    currentEvent++;
            }
        }

        public void ActivateEvent(SessionEvent eventData)
        {
            switch (eventData.EventType)
            {
                case SessionEventType.StartWave:
                    {
                        EnemySpawner.Instance.StartSpawning();
                        break;
                    }
                case SessionEventType.StartMicroWave:
                    {
                        EnemySpawner.Instance.StartSpawning(eventData.PercentFromScore);
                        break;
                    }
                default:
                    break;
            }
        }

        private void OnGamePaused(bool enabled)
        {
            if (enabled)
                MusicAudioSource.Pause();
            else
                MusicAudioSource.UnPause();
        }

        private void OnDisable()
        {
            Pause.Instance.OnGamePaused += OnGamePaused;
        }
    }
}
