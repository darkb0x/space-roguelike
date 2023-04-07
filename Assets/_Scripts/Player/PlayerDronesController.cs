using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    using Drone;

    public class PlayerDronesController : MonoBehaviour
    {
        public class DroneOrbit
        {
            public float Dir;
            public int I;
            public float Distance;
        }

        private Dictionary<DroneOrbit, List<DroneAI>> Drones = new Dictionary<DroneOrbit, List<DroneAI>>();

        [Header("Variables")]
        [SerializeField] private int MaxDroneAmountInOrbit;
        [SerializeField] private float DroneDistanceFromPlayer;
        [SerializeField] private float DroneRotationSpeed;

        private Transform myTransform;

        private void Start()
        {
            myTransform = transform;

            Drones.Add(new DroneOrbit() { Dir = 0, I = 0, Distance = DroneDistanceFromPlayer }, new List<DroneAI>());
        }

        private void Update()
        {
            UpdateDroneRotation();
        }

        private void UpdateDroneRotation()
        {
            foreach (var droneOrbit in Drones.Keys)
            {
                droneOrbit.Dir += Time.deltaTime * DroneRotationSpeed;

                foreach (var drone in Drones[droneOrbit])
                {
                    drone.RotationUpdate(myTransform, (droneOrbit.I / (float)(Drones[droneOrbit].Count)) * 360f + droneOrbit.Dir, droneOrbit.Distance);
                    droneOrbit.I++;
                }
            }
        }
        

        public void AttachDrone(DroneAI drone)
        {
            foreach (var orbit in Drones.Keys)
            {
                if (Drones[orbit].Count >= MaxDroneAmountInOrbit)
                    continue;

                Drones[orbit].Add(drone);
                return;
            }

            // If all orbits is full
            Drones.Add(new DroneOrbit { Dir = 0, I = 0, Distance = Drones.Keys.Count+1 * DroneDistanceFromPlayer }, 
                       new List<DroneAI> { drone }
                       );
        }

        public void DetachDrone(DroneAI drone)
        {
            foreach (var orbit in Drones.Keys)
            {
                if(Drones[orbit].Contains(drone))
                {
                    Drones[orbit].Remove(drone);
                    return;
                }
            }
        }
    }
}
