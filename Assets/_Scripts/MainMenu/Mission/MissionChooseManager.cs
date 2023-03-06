using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace Game.MainMenu.Mission
{
    using Planet;
    using Planet.Visual;
    using Visual;

    public class MissionChooseManager : MonoBehaviour
    {
        public static MissionChooseManager Instance;

        [Header("Visual")]
        [SerializeField] private MissionChooseVisual Visual;
        [SerializeField] private PlanetUIVisual PlanetVisualPrefab;
        [SerializeField] private Canvas Canvas;

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

            startMissionTimer -= Time.deltaTime;

            Visual.ShowStartMissionTimer(startMissionTimer);

            if (startMissionTimer <= 0)
                StartMission();
        }

        public void SelectMission(PlanetSO mission)
        {
            selectedMission = mission;

            Visual.ShowMissionTab(mission.MissionIcon, mission.MissionName);

            Visual.CloseMenu();
        }

        public void StartMission()
        {
            SceneManager.LoadScene(selectedMission.SceneId);
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
