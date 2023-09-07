using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using NaughtyAttributes;

namespace Game.MainMenu.MissionChoose
{
    using SceneLoading;
    using Planet;
    using Planet.Visual;
    using Visual;
    using Save;
    using Audio;

    public class MissionChooseManager : MonoBehaviour, IService
    {
        [Header("Visual")]
        [SerializeField] private MissionChooseVisual Visual;
        [SerializeField] private PlanetUIVisual PlanetVisualPrefab;
        [SerializeField] private Canvas Canvas;
        [Space]
        [SerializeField] private PlayableDirector Cutscene;

        [Header("Planets")]
        [SerializeField, Expandable] private PlanetMapSO PlanetMap;
        [SerializeField] private Transform CentralPoint;

        [field: Header("Variables")]
        [field: SerializeField, Label("Start Mission Timer")] public float m_StartMissionTimer { get; private set; }

        public System.Action<PlanetSO> OnMissionSelected;

        private Dictionary<Orbit, List<PlanetUIVisual>> allPlanetsOnUI = new Dictionary<Orbit, List<PlanetUIVisual>>();
        private Dictionary<Orbit, float> planetDirection = new Dictionary<Orbit, float>();
        private Dictionary<Orbit, int> planetProgress = new Dictionary<Orbit, int>();
        private PlanetSO selectedMission;

        private bool startMission = false;
        public float startMissionTimer { get; private set; }

        private SessionSaveData currentSessionData => SaveManager.SessionSaveData;

        public void Initialize()
        {
            startMissionTimer = 0; 
            
            // Initalize planets
            for (int i = 0; i < PlanetMap.Orbits.Count; i++)
            {
                Orbit orbit = PlanetMap.Orbits[i];

                allPlanetsOnUI.Add(orbit, new List<PlanetUIVisual>());

                planetDirection.Add(orbit, 0);
                planetProgress.Add(orbit, 0);

                foreach (var planet in orbit.PlanetsOnOrbit)
                {
                    PlanetUIVisual visual = Instantiate(PlanetVisualPrefab.gameObject, CentralPoint).GetComponent<PlanetUIVisual>();
                    visual.Initialize(planet);

                    allPlanetsOnUI[orbit].Add(visual);
                }
            }

            Visual.HideMissionTab();
        }

        private void Update()
        {
            UpdateOrbit();
            UpdateStartMissionTimer();
        }

        private void UpdateOrbit()
        {
            foreach (var orbit in PlanetMap.Orbits)
            {
                planetDirection[orbit] += Time.deltaTime * orbit.Speed;

                foreach (var planet in allPlanetsOnUI[orbit])
                {
                    planet.Rotate(CentralPoint, ((float)planetProgress[orbit] / (float)orbit.PlanetsOnOrbit.Count) * 360f + planetDirection[orbit], orbit.DistanceFromPoint * Canvas.scaleFactor);
                    planetProgress[orbit]++;
                }
            }
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

            Visual.ShowMissionTab(mission.MissionIcon, mission.MissionName, mission.UniqueItems);

            Visual.CloseMenu();
        }

        public void StartCutscene()
        {
            ServiceLocator.GetService<UIPanelManager>().CloseAllPanel(false);

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
