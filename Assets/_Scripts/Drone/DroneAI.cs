using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DroneAI : MonoBehaviour
{
    [Header("States")]
    [SerializeField, Expandable] private DroneAction action;

    [Header("Render")]
    public Transform spriteTransform;

    [Header("Other")]
    [ReadOnly] public PlayerDronesController playerDrCo;
    [ReadOnly] public Ore targetOre;
    [ReadOnly] public EnemyAI targetEnemy;
    public bool isPicked = false;

    DroneAction currentAction;
    PlayerController player;
    Transform playerTransform;
    [HideInInspector] public Rigidbody2D rb;
    new Transform transform;
    

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        playerDrCo = FindObjectOfType<PlayerDronesController>();
        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        playerTransform = player.transform;
    }

    public void Init()
    {
        isPicked = true;
        SetAction(action);
    }

    public void Update()
    {
        if(isPicked) currentAction.Run();
    }
    public void FixedUpdate()
    {
        if(isPicked) currentAction.FixedRun();
    }

    public void RotationUpdate(Transform point, float direction, float rangeFromPoint)
    {
        transform.position = (Vector2)point.position + new Vector2(Mathf.Sin(direction * Mathf.Deg2Rad), Mathf.Cos(direction * Mathf.Deg2Rad)) * rangeFromPoint;
    }

    public Vector2 GetReturnPos()
    {
        float direction = playerDrCo.GetDegressValue();
        Vector2 pos = (Vector2)playerDrCo.transform.position + new Vector2(Mathf.Sin(direction * Mathf.Deg2Rad), Mathf.Cos(direction * Mathf.Deg2Rad)) * playerDrCo.distance;
        return pos;
    }

    public void SetAction(DroneAction d)
    {
        currentAction = Instantiate(d);
        currentAction.player = player;
        currentAction.drone = this;
        currentAction.Init();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Ore>(out Ore o))
        {
            targetOre = o;
        }

        if (collision.TryGetComponent<EnemyAI>(out EnemyAI e))
        {
            targetEnemy = e;
        }

        action.TriggerEnter(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        action.TriggerStay(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        action.TriggerExit(collision);
    }

    private void OnDestroy()
    {
        playerDrCo.DetachDrone(this);
    }
}
