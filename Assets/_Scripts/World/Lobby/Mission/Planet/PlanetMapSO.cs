using System.Collections.Generic;
using UnityEngine;

namespace Game.Lobby.Missions
{
    [System.Serializable]
    public class Orbit
    {
        public List<PlanetSO> PlanetsOnOrbit = new List<PlanetSO>();
        public float DistanceFromPoint;
        public float Speed;
    }

    [CreateAssetMenu(fileName = "Planet map", menuName = "Game/Mission/new Planet map", order = 0)]
    public class PlanetMapSO : ScriptableObject
    {
        public List<Orbit> Orbits = new List<Orbit>();
    }
}
