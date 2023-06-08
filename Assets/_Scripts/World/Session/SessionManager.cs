using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using NaughtyAttributes;

namespace Game.Session
{
    using Enemy;
    using MainMenu.MissionChoose.Planet;
    using MainMenu.Pause;
    using SaveData;
    using Utilities.LoadScene;
    using Audio;

    public enum SessionEventType
    {
        StartMicroWave,
        StartWave
    }

    public class SessionManager : MonoBehaviour, ISingleton
    {
        [Header("Rocket")]
        [SerializeField] private Transform StartRocket;
        [SerializeField] private Transform[] RocketPositions;

        [Header("Session")]
        [Expandable] public SessionTimingsSO SessionTimings;
        [SerializeField] private PlayableDirector EndCutscene;

        [Header("Audio")]
        [SerializeField] private AudioClip Music;

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

        private EnemySpawner EnemySpawner;
        public System.Action<SessionEvent> OnEventReached;

        private void Awake()
        {
            Singleton.Add(this);

            eventsCount = SessionTimings.EventsList.Count;
            currentEvent = 0;

            StartRocket.position = RocketPositions[Random.Range(0, RocketPositions.Length)].position;

            LogUtility.StartLogging("session");

            #if !UNITY_EDITOR
            DebugText.gameObject.SetActive(false);
            #endif
        }

        private void Start()
        {
            EnemySpawner = Singleton.Get<EnemySpawner>();
            Singleton.Get<PauseManager>().OnGamePaused += OnGamePaused;

            planetData = SaveDataManager.Instance.CurrentSessionData.GetPlanet();
        }

        public void Initialize()
        {
            MusicManager.Instance.SetMusic(Music, false);

            isPlaying = true;
        }

        private void Update()
        {
            if (!isPlaying)
                return;
            currentTime += Time.deltaTime * speed;

            DebugText.text = $"Time: {currentTime}\n" +
                             $"Event: {currentEvent}";

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
            SaveDataManager.Instance.CurrentSessionData.CurrentDifficultFactor += difficultAddition;
        }
        public void EnableLobbyScene()
        {
            SaveDataManager.Instance.CurrentSessionData.Save();

            LoadSceneUtility.EnableLoadedAsyncScene();
        }

        public void SetPlayerIntoRocket(Transform parent)
        {
            FindObjectOfType<Player.PlayerController>().LockPlayerPosition(parent);
        }

        private void OnDisable()
        {
            LogUtility.StopLogging();
            Singleton.Get<PauseManager>().OnGamePaused -= OnGamePaused;
        }
    }
}
