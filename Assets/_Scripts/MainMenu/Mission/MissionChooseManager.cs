using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using NaughtyAttributes;

namespace Game.MainMenu.Mission
{
    using Utilities.LoadScene;
    using Utilities;
    using Planet;
    using Planet.Visual;
    using Visual;
    using SaveData;

    public class MissionChooseManager : MonoBehaviour
    {
        public static MissionChooseManager Instance;

        [Header("Visual")]
        [SerializeField] private MissionChooseVisual Visual;
        [SerializeField] private PlanetUIVisual PlanetVisualPrefab;
        [SerializeField] private Canvas Canvas;
        [Space]
        [SerializeField] private PlayableDirector Cutscene;

        [Header("Planets")]
        [SerializeField, Expandable] private PlanetMapSO PlanetMap;
        [SerializeField] private Transform CentralPoint;

        [Header("Variables")]
        [SerializeField] private float m_StartMissionTimer;

        private Dictionary<Orbit, List<PlanetUIVisual>> allPlanetsOnUI = new Dictionary<Orbit, List<PlanetUIVisual>>();
        private Dictionary<Orbit, float> planetDirection = new Dictionary<Orbit, float>();
        private Dictionary<Orbit, int> planetProgress = new Dictionary<Orbit, int>();
        private PlanetSO selectedMission;

        private bool startMission = false;
        private float startMissionTimer;

        private SessionData currentSessionData => GameData.Instance.CurrentSessionData;

        private void Awake()
        {
            Instance = this;

            startMissionTimer = m_StartMissionTimer;
        }

        private void Start()
        {
            // Initialize planets
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
            Visual.HideStartMissionTimer();
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
                startMissionTimer = m_StartMissionTimer;

                Visual.HideStartMissionTimer();

                return;
            }

            startMissionTimer = Mathf.Clamp(startMissionTimer - Time.deltaTime, 0, m_StartMissionTimer);

            Visual.ShowStartMissionTimer(startMissionTimer);

            if (startMissionTimer <= 0)
                StartCutscene();
        }

        public void SelectMission(PlanetSO mission)
        {
            if(selectedMission != mission)
            {
                selectedMission = mission;
            }

            Visual.ShowMissionTab(mission.MissionIcon, mission.MissionName, mission.UniqueItems);

            Visual.CloseMenu();
        }

        public void StartCutscene()
        {
            UIPanelManager.Instance.CloseAllPanel(false);

            Cutscene.Play();
            currentSessionData.SetPlanet(selectedMission.AssetPath);
            StartCoroutine(LoadSceneUtility.Instance.LoadSceneAsync(selectedMission.SceneId, 20));

            currentSessionData.Save();
        }
        public void StartMission()
        {
            LogUtility.WriteLog($"Start mission: {selectedMission.MissionName}");

            LoadSceneUtility.Instance.EnableLoadedAsyncScene();
        }
        public void StartMissionTimer()
        {
            if (selectedMission == null)
                return;

            startMission = true;
        }
        public void StopMissionTimer()
        {
            startMission = false;
        }
    }
}
