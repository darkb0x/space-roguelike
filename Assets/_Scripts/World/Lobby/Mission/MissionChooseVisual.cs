using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Lobby.Missions
{
    using Input;
    using UI;

    public class MissionChooseVisual : Window
    {
        public const string PLANET_VISUAL_PATH = "Prefabs/UI/Elements/MissionScreen/Planet";

        [Header("Main")]
        [SerializeField] private Transform Content;
        [SerializeField] private Transform CentralPlanet;

        public override WindowID ID => MissionChooseManager.MISSION_SCREEN_WINDOW_ID;

        private UIInputHandler _input => InputManager.UIInputHandler;

        private PlanetMapSO _planetMap;
        private MissionTabHUD _missionTab;
        private Dictionary<Orbit, List<PlanetUIVisual>> _allPlanets = new Dictionary<Orbit, List<PlanetUIVisual>>();
        private Dictionary<Orbit, float> _planetsDirection = new Dictionary<Orbit, float>();
        private Dictionary<Orbit, int> _planetsProgress = new Dictionary<Orbit, int>();

        private Vector2 defaultContentPosition;

        public void Initialize(PlanetMapSO mapSO)
        {
            _planetMap = mapSO;

            defaultContentPosition = Content.position;

            InitializeMissionMap();
        }
        private void InitializeMissionMap()
        {
            var planetPrefab = Resources.Load<PlanetUIVisual>(PLANET_VISUAL_PATH);

            for (int i = 0; i < _planetMap.Orbits.Count; i++)
            {
                Orbit orbit = _planetMap.Orbits[i];

                _allPlanets.Add(orbit, new List<PlanetUIVisual>());

                _planetsDirection.Add(orbit, 0);
                _planetsProgress.Add(orbit, 0);

                foreach (var planet in orbit.PlanetsOnOrbit)
                {
                    PlanetUIVisual visual = Instantiate(planetPrefab, CentralPlanet);
                    visual.Initialize(planet);

                    _allPlanets[orbit].Add(visual);
                }
            }
        }

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();

            _input.CloseEvent += _closeAction;
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();

            _input.CloseEvent -= _closeAction;
        }

        private void Update()
        {
            if (!IsOpened)
                return;

            UpdateOrbits();
        }

        public override void Open(bool notify = true)
        {
            Content.position = defaultContentPosition;
            base.Open(notify);
        }

        private void UpdateOrbits()
        {
            foreach (var orbit in _planetMap.Orbits)
            {
                _planetsDirection[orbit] += Time.deltaTime * orbit.Speed;

                foreach (var planet in _allPlanets[orbit])
                {
                    planet.Rotate(CentralPlanet, ((float)_planetsProgress[orbit] / (float)orbit.PlanetsOnOrbit.Count) * 360f + _planetsDirection[orbit], orbit.DistanceFromPoint * _uiWindowService.RootCanvas.scaleFactor);
                    _planetsProgress[orbit]++;
                }
            }
        }
    }
}
