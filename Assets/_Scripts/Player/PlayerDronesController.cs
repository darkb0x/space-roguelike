using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDronesController : MonoBehaviour
{
    private List<DroneAI> rotateDrones = new List<DroneAI>();

    [Header("miners")]
    [SerializeField] private List<DroneAI> miners = new List<DroneAI>();

    [Header("protectors")]
    [SerializeField] private List<DroneAI> protectors = new List<DroneAI>();

    [Header("attackers")]
    [SerializeField] private List<DroneAI> attackers = new List<DroneAI>();

    [Header("properties")]
    [SerializeField] private float finderClickRadius = 0.3f;
    [SerializeField] private LayerMask layers;
    [Space]
    public float droneRotationSpeed;
    public float distance = 2.3f;
    public bool doRotate = true;

    Camera cam;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, finderClickRadius);
    }

    private void Start()
    {
        cam = Camera.main;
    }

    float dir = 0;
    int i = 0;

    public float GetDegressValue()
    {
        return (i / (float)(rotateDrones.Count + 1) * 360 + dir);
    }

    private void Update()
    {
        if(doRotate) dir += Time.deltaTime * droneRotationSpeed;

        foreach (var rotator in rotateDrones)
        {
            rotator.RotationUpdate(transform, (i / (float)(rotateDrones.Count)) * 360 + dir, distance);
            i++;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] colls = Physics2D.OverlapCircleAll(mousePos, finderClickRadius, layers);
            foreach (var item in colls)
            {
                if(item.TryGetComponent<Ore>(out Ore ore))
                {
                    foreach (var drone in miners)
                    {
                        drone.targetOre = ore;
                    }
                }

                if(item.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    foreach (var drone in attackers)
                    {
                        drone.targetEnemy = enemy;
                    }
                }
            }
        }
    }

    public void AttachMinerDrone(DroneAI drone)
    {
        if (!miners.Contains(drone))
            miners.Add(drone);
    }
    public void AttachProtectorDrone(DroneAI drone)
    {
        if (!protectors.Contains(drone))
            protectors.Add(drone);
    }
    public void AttachAttackerDrone(DroneAI drone)
    {
        if (!attackers.Contains(drone))
            attackers.Add(drone);
    }
    public void DetachDrone(DroneAI drone)
    {
        if (miners.Contains(drone))
            miners.Remove(drone);

        if (protectors.Contains(drone))
            protectors.Remove(drone);

        if (attackers.Contains(drone))
            attackers.Remove(drone);
    }

    public void AttachRotateDrones(DroneAI drone)
    {
        if (!rotateDrones.Contains(drone))
            rotateDrones.Add(drone);
    }
    public void DetachRotateDrones(DroneAI drone)
    {
        if (rotateDrones.Contains(drone))
            rotateDrones.Remove(drone);
    }
}
