using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    using Drone;
    using World.Generation.Ore;

    public class PlayerDronesController : MonoBehaviour
    {
        public class DroneOrbit
        {
            public float Dir;
            public int I;
            public float Distance;
            public int Index;

            public DroneOrbit(float distance, int index)
            {
                Dir = 0;
                I = 0;
                Distance = distance;
                Index = index;
            }
        }

        private Dictionary<DroneOrbit, List<DroneAI>> drones = new Dictionary<DroneOrbit, List<DroneAI>>();

        [Header("Variables")]
        [SerializeField] private int MaxDroneAmountInOrbit;
        [SerializeField] private float DroneDistanceFromPlayer;
        [SerializeField] private float DroneRotationSpeed;

        [Header("Miner")]
        [SerializeField] private LayerMask OreLayer;
        [SerializeField] private List<DroneMiner> DroneMiners = new List<DroneMiner>();

        private Camera cam;
        private Transform myTransform;

        private void Start()
        {
            myTransform = transform;
            cam = Camera.main;

            drones.Add(new DroneOrbit(DroneDistanceFromPlayer, 1), new List<DroneAI>());
        }

        private void Update()
        {
            if(Mouse.current.leftButton.isPressed)
            {
                Vector2 mousePos = cam.ScreenToWorldPoint(GameInput.Instance.GetMousePosition());

                if(DroneMiners.Count > 0)
                {
                    SetOreTarget(mousePos);
                }
            }

            UpdateDroneRotation();
        }

        private void SetOreTarget(Vector2 mousePos)
        {
            float oreFinderRadius = 0.5f;
            DroneMiner freeDrone = null;

            foreach (var drone in DroneMiners)
            {
                if (drone.currentOre == null)
                {
                    freeDrone = drone;
                    break;
                }
            }

            if(freeDrone == null)
            {
                freeDrone = DroneMiners[Random.Range(0, DroneMiners.Count)];
            }

            Collider2D[] oreColls = Physics2D.OverlapCircleAll(mousePos, oreFinderRadius, OreLayer);
            foreach (var oreColl in oreColls)
            {
                if(oreColl.TryGetComponent<Ore>(out Ore ore))
                {                    
                    if(freeDrone.SetOre(ore))
                    {
                        break;
                    }

                    
                    if (ore.currentDrone != null)
                    {
                        DroneMiner droneMiner = ore.currentDrone as DroneMiner;
                        droneMiner.SetFree();

                        ore.currentDrone = null;

                        break;
                    }
                }
            }
        }

        private void UpdateDroneRotation()
        {
            foreach (var droneOrbit in drones.Keys)
            {
                int rotationSide = droneOrbit.Index % 2 == 0 ? 1 : -1;
 
                droneOrbit.Dir += Time.deltaTime * DroneRotationSpeed * rotationSide;

                foreach (var drone in drones[droneOrbit])
                {
                    drone.RotationUpdate(myTransform, (droneOrbit.I / (float)(drones[droneOrbit].Count)) * 360f + droneOrbit.Dir, droneOrbit.Distance);
                    droneOrbit.I++;
                }
            }
        }
        

        public void AttachDrone(DroneAI drone)
        {
            if(drone is DroneMiner droneMiner)
            {
                DroneMiners.Add(droneMiner);
            }

            foreach (var orbit in drones.Keys)
            {
                if (drones[orbit].Count >= MaxDroneAmountInOrbit)
                    continue;

                drones[orbit].Add(drone);
                return;
            }

            // If all orbits is full
            drones.Add(new DroneOrbit(drones.Keys.Count + 1 * DroneDistanceFromPlayer, drones.Keys.Count + 1), 
                       new List<DroneAI> { drone }
                       );
        }

        public void DetachDrone(DroneAI drone)
        {
            if (drone is DroneMiner droneMiner)
            {
                DroneMiners.Remove(droneMiner);
            }

            foreach (var orbit in drones.Keys)
            {
                if(drones[orbit].Contains(drone))
                {
                    drones[orbit].Remove(drone);
                    return;
                }
            }
        }
    }
}
