using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Game
{
    using Audio;
    using Enemy;
    using MainMenu.Mission.Planet;
    using MainMenu.Pause;
    using SaveData;

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

        private bool isPlaying = false;

        public PlanetSO planetData { get; private set; }

        private void Awake()
        {
            Instance = this;

            eventsCount = SessionTimings.EventsList.Count;
            currentEvent = 0;
        }

        private void Start()
        {
            PauseManager.Instance.OnGamePaused += OnGamePaused;

            planetData = GameData.Instance.CurrentSessionData.Planet;
        }

        public void Initialize()
        {
            MusicAudioSource.clip = SessionTimings.Music;
            MusicAudioSource.Play();

            isPlaying = true;
        }

        private void Update()
        {
            if (!isPlaying)
                return;

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
            PauseManager.Instance.OnGamePaused += OnGamePaused;
        }
    }
}
