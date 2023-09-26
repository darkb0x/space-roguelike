using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using NaughtyAttributes;

namespace Game.Session
{
    using Enemy;
    using Lobby.Missions;
    using Menu.Pause;
    using Save;
    using SceneLoading;
    using Audio;
    using Artefact;

    public enum SessionEventType
    {
        StartMicroWave,
        StartWave
    }

    public class SessionManager : MonoBehaviour, IService, IEntryComponent<EnemySpawner, PauseManager>
    {
        [Header("Rocket")]
        [SerializeField] private Transform StartRocket;
        [SerializeField] private Transform[] RocketPositions;

        [Header("Session")]
        public ArtefactDrill ArtefactDrill;
        [Space]
        [Expandable] public SessionTimingsSO SessionTimings;
        [SerializeField] private PlayableDirector EndCutscene;

        [Header("Audio")]
        [SerializeField] private AudioClip Music;

        [Header("Debug")]
        [ReadOnly] public float currentTime;
        [SerializeField] private float speed = 1;

        [Header("End")]
        [SerializeField, Scene] private int LobbySceneID = 1;

        private int eventsCount = 0;
        private int currentEvent = 0;

        private bool isPlaying = false;

        public PlanetSO planetData { get; private set; }

        private SessionSaveData _sessionSave => SaveManager.SessionSaveData;
        private EnemySpawner EnemySpawner;
        private PauseManager _pauseManager;
        public System.Action<SessionEvent> OnEventReached;

        public void Initialize(EnemySpawner enemySpawner, PauseManager pauseManager)
        {  
            EnemySpawner = enemySpawner;
            _pauseManager = pauseManager;

            eventsCount = SessionTimings.EventsList.Count;
            currentEvent = 0;
            planetData = _sessionSave.GetPlanet();

            StartRocket.position = RocketPositions[Random.Range(0, RocketPositions.Length)].position;

            LogUtility.StartLogging("session");
            Debug.Log($"Loaded mission:{planetData.MissionName}, on difficulty:{_sessionSave.CurrentDifficultFactor}");

            _pauseManager.OnGamePaused += OnGamePaused;

            ArtefactDrill.Initialize();

            #if !UNITY_EDITOR
            DebugText.gameObject.SetActive(false);
            #endif
        }

        public void StartPlaying()
        {
            MusicManager.Instance.SetMusic(Music, false);

            isPlaying = true;
        }

        private void Update()
        {
            if (!isPlaying)
                return;
            currentTime += Time.deltaTime * speed;

            if (currentEvent >= eventsCount)
                return;

            if (currentTime >= SessionTimings.EventsList[currentEvent].StartTime)
            {
                ActivateEvent(SessionTimings.EventsList[currentEvent]);

                if (currentEvent < eventsCount)
                    currentEvent++;
            }
        }

        public void ActivateEvent(SessionEvent eventData)
        {
            Debug.Log($"Event Activated {currentEvent} - {eventData.EventType}. Time: {currentTime}");
            switch (eventData.EventType)
            {
                case SessionEventType.StartWave:
                    {
                        SessionEvent.WaveDuration waveData = eventData.WaveTime;

                        EnemySpawner.StartWave(waveData.StartTime, waveData.EndTime);
                        break;
                    }
                case SessionEventType.StartMicroWave:
                    {
                        EnemySpawner.StartSpawning(eventData.PercentFromScore);
                        break;
                    }
                default:
                    break;
            }
            OnEventReached?.Invoke(eventData);
        }

        private void OnGamePaused(bool enabled)
        {
            if (enabled)
                MusicManager.Instance.AudioSource.Pause();
            else
                MusicManager.Instance.AudioSource.UnPause();
        }

        public void StartLoadingLobby()
        {
            LoadSceneUtility.LoadSceneAsync(LobbySceneID);
            EndCutscene.Play();

            float difficultAddition = 0.05f;
            _sessionSave.CurrentDifficultFactor += difficultAddition;
        }
        public void EnableLobbyScene()
        {
            LoadSceneUtility.EnableLoadedAsyncScene();
        }

        public void SetPlayerIntoRocket(Transform parent)
        {
            ServiceLocator.GetService<CutsceneManager>().LockPlayerPosition(parent);
        }

        private void OnDisable()
        {
            LogUtility.StopLogging();
            _pauseManager.OnGamePaused -= OnGamePaused;
        }
    }
}
