using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDronesController : MonoBehaviour
{
    [Header("miners")]
    [SerializeField] private List<DroneAI> miners = new List<DroneAI>();

    [Header("protectors")]
    [SerializeField] private List<DroneAI> protectors = new List<DroneAI>();

    [Header("properties")]
    [SerializeField] private float radius = 0.3f;
    [SerializeField] private LayerMask layers;

    Camera cam;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] colls = Physics2D.OverlapCircleAll(mousePos, radius, layers);
            foreach (var item in colls)
            {
                if(item.TryGetComponent<Ore>(out Ore ore))
                {
                    foreach (var drone in miners)
                    {
                        if (!drone.targetOre)
                            drone.targetOre = ore;
                    }
                }
            }
        }
    }

    public void AttachMinerDrone(DroneAI drone)
    {
        miners.Add(drone);
    }
    public void AttachProtectorDrone(DroneAI drone)
    {
        protectors.Add(drone);
    }
    public void DetachDrone(DroneAI drone)
    {
        if (miners.Contains(drone))
            miners.Remove(drone);

        if (protectors.Contains(drone))
            protectors.Remove(drone);
    }
}
