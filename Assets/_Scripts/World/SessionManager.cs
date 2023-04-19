using UnityEngine;
using TMPro;
using NaughtyAttributes;

namespace Game
{
    using Audio;
    using Enemy;
    using MainMenu.Mission.Planet;
    using MainMenu.Pause;
    using SaveData;
    using Utilities.LoadScene;

    public enum SessionEventType
    {
        StartMicroWave,
        StartWave
    }

    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance;

        [Header("Rocket")]
        [SerializeField] private Transform Rocket;
        [SerializeField] private Transform[] RocketPositions;

        [Header("Session Theme")]
        [SerializeField, Expandable] private SessionTimingsSO SessionTimings;

        [Header("Audio")]
        [SerializeField] private AudioSource MusicAudioSource;

        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI DebugText;
        [ReadOnly] public float currentTime;
        [SerializeField] private float speed = 1;

        [Header("End")]
        [SerializeField, Scene] private int LobbySceneID = 1;

        private int eventsCount = 0;
        private int currentEvent = 0;

        private bool isPlaying = false;

        public PlanetSO planetData { get; private set; }

        private void Awake()
        {
            Instance = this;

            eventsCount = SessionTimings.EventsList.Count;
            currentEvent = 0;

            Rocket.position = RocketPositions[Random.Range(0, RocketPositions.Length)].position;

            LogUtility.StartLogging("session");

            #if !UNITY_EDITOR
            DebugText.gameObject.SetActive(false);
            #endif
        }

        private void Start()
        {
            PauseManager.Instance.OnGamePaused += OnGamePaused;

            planetData = GameData.Instance.CurrentSessionData.GetPlanet();
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

            currentTime += Time.deltaTime * speed;

            if (currentTime >= SessionTimings.EventsList[currentEvent].StartTime)
            {
                ActivateEvent(SessionTimings.EventsList[currentEvent]);

                if (currentEvent < eventsCount)
                    currentEvent++;
            }
        }

        public void ActivateEvent(SessionEvent eventData)
        {
            Debug.Log($"Event Activated {currentEvent} - {eventData.EventType}");
            switch (eventData.EventType)
            {
                case SessionEventType.StartWave:
                    {
                        SessionEvent.WaveDuration waveData = eventData.WaveTime;

                        EnemySpawner.Instance.StartWave(waveData.StartTime, waveData.EndTime);
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

        public void StartLoadingLobby()
        {
            LoadSceneUtility.Instance.LoadSceneAsync(LobbySceneID);
        }
        public void EnableLobbyScene()
        {
            GameData.Instance.CurrentSessionData.Save();

            LoadSceneUtility.Instance.EnableLoadedAsyncScene();
        }

        public void SetPlayerIntoRocket(Transform parent)
        {
            FindObjectOfType<Player.PlayerController>().LockPlayerPosition(parent);
        }

        private void OnDisable()
        {
            LogUtility.StopLogging();
            PauseManager.Instance.OnGamePaused += OnGamePaused;
        }
    }
}
