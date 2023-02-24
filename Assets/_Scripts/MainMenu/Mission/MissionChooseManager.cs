using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MainMenu.Mission
{
    using Planet;

    public class MissionChooseManager : MonoBehaviour
    {
        [Header("Planets")]
        [SerializeField] private PlanetMapSO PlanetMap;
        [SerializeField] private Transform CentralPoint;

        [Header("UI Visual")]
        [SerializeField] private PlanetUIVisual PlanetVisual;

        private Dictionary<Orbit, List<PlanetUIVisual>> allPlanetsOnUI = new Dictionary<Orbit, List<PlanetUIVisual>>();

        private Dictionary<Orbit, float> planetDirection = new Dictionary<Orbit, float>();
        private Dictionary<Orbit, int> planetProgress = new Dictionary<Orbit, int>();

        private void Start()
        {
            for (int i = 0; i < PlanetMap.Orbits.Count; i++)
            {
                Orbit orbit = PlanetMap.Orbits[i];

                allPlanetsOnUI.Add(orbit, new List<PlanetUIVisual>());

                planetDirection.Add(orbit, 0);
                planetProgress.Add(orbit, 0);

                foreach (var planet in orbit.PlanetsOnOrbit)
                {
                    PlanetUIVisual visual = Instantiate(PlanetVisual.gameObject, CentralPoint).GetComponent<PlanetUIVisual>();
                    visual.Initialize(planet);

                    allPlanetsOnUI[orbit].Add(visual);
                }
            }
        }

        private void Update()
        {
            UpdateOrbit();
        }

        private void UpdateOrbit()
        {
            foreach (var orbit in PlanetMap.Orbits)
            {
                planetDirection[orbit] += Time.deltaTime * orbit.Speed;

                foreach (var planet in allPlanetsOnUI[orbit])
                {
                    planet.Rotate(CentralPoint, (planetProgress[orbit] / (float)(orbit.PlanetsOnOrbit.Count)) * 360f + planetDirection[orbit], orbit.DistanceFromPoint);
                    planetProgress[orbit]++;
                }
            }
        }
    }
}
