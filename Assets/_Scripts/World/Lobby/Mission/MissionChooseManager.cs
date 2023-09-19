using UnityEngine;
using UnityEngine.Playables;
using NaughtyAttributes;
using System;

namespace Game.Lobby.Missions
{
    using SceneLoading;
    using Save;
    using Audio;
    using UI;

    public class MissionChooseManager : MonoBehaviour, IService, IEntryComponent<UIWindowService>
    {
        public const WindowID MISSION_SCREEN_WINDOW_ID = WindowID.MissionSelect; 

        [Header("Visual")]
        [SerializeField] private PlayableDirector Cutscene;

        [Header("Planets")]
        [SerializeField, Expandable] private PlanetMapSO PlanetMap;

        [field: Header("Variables")]
        [field: SerializeField, Label("Start Mission Timer")] public float m_StartMissionTimer { get; private set; }

        public Action<PlanetSO> OnMissionSelected;
        public Action OnMissionTabShowed;
        public Action OnMissionTabHided;

        private SessionSaveData currentSessionData => SaveManager.SessionSaveData;
        private UIWindowService _uiWindowService;
        private MissionChooseVisual _visual;

        private PlanetSO selectedMission;

        private bool startMission = false;
        public float startMissionTimer { get; private set; }

        public void Initialize(UIWindowService windowService)
        {
            _uiWindowService = windowService;

            startMissionTimer = 0;

            _visual = _uiWindowService.RegisterWindow<MissionChooseVisual>(MISSION_SCREEN_WINDOW_ID);
            _visual.Initialize(PlanetMap);
        }

        private void Update()
        {
            UpdateStartMissionTimer();
        }
        private void UpdateStartMissionTimer()
        {
            if(!startMission)
            {
                startMissionTimer = Mathf.Lerp(startMissionTimer, 0, 7f * Time.deltaTime); // 7f - speed 

                return;
            }

            startMissionTimer = Mathf.Clamp(startMissionTimer + Time.deltaTime, 0, m_StartMissionTimer);

            if (startMissionTimer == m_StartMissionTimer)
                StartCutscene();
        }

        public void SelectMission(PlanetSO mission)
        {
            if(selectedMission != mission)
            {
                selectedMission = mission;
                OnMissionSelected?.Invoke(mission);
            }

            OnMissionTabShowed?.Invoke();

            _uiWindowService.Close(MISSION_SCREEN_WINDOW_ID);
        }

        public void StartCutscene()
        {
            ServiceLocator.GetService<UIWindowService>().CloseAll();

            Cutscene.Play();
            currentSessionData.SetPlanet(selectedMission);
            LoadSceneUtility.LoadSceneAsync(selectedMission.SceneId, 20);
        }
        public void StartMission()
        {
            LogUtility.WriteLog($"Start mission: {selectedMission.MissionName}");

            LoadSceneUtility.EnableLoadedAsyncScene();

            MusicManager.Instance.SetMusic(null, false);
        }
        public void StartMissionTimer()
        {
            if (selectedMission == null)
                return;

            startMission = true;
        }
        public void StopMissionTimer()
        {
            if (startMissionTimer <= 0)
                return;

            startMission = false;
        }
    }
}
