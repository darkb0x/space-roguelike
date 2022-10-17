using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAI : MonoBehaviour
{
    [Header("States")]
    [SerializeField, NaughtyAttributes.Expandable] private DroneAction action;

    DroneAction currentAction;
    PlayerController player;
    Transform playerTransform;
    [HideInInspector] public Rigidbody2D rb;
    new Transform transform;
    [HideInInspector] public Ore targetOre;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
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
}
