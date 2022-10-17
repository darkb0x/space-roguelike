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

        SetAction(action);
    }

    public void Update()
    {
        currentAction.Run();
    }
    public void FixedUpdate()
    {
        currentAction.FixedRun();
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
