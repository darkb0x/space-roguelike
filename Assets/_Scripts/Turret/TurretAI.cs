using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [Header("parameters")]
    public GameObject bulletPrefab;
    public int damage;
    public Transform shotPos;
    public Transform turret_canon;
    public float timeBtwAttack = 0.3f;

    [Header("actions")]
    [SerializeField, NaughtyAttributes.Expandable] private TurretAction action;
    [NaughtyAttributes.ReadOnly] public GameObject currentEnemy;
    [NaughtyAttributes.ReadOnly] public bool enemyInZone;
    [NaughtyAttributes.Tag] public string enemyTag;

    [Header("other")]
    public bool isPicked;
    [SerializeField] private Collider2D coll;

    TurretAction currentAction;
    PlayerController player;
    PlayerInventory inventory;
    new Transform transform;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        inventory = FindObjectOfType<PlayerInventory>();
        transform = GetComponent<Transform>();

        SetAction(action);

        coll.enabled = false;
    }

    public void Put()
    {
        isPicked = false;
        coll.enabled = true;
    }

    private void Update()
    {
        if (!isPicked) currentAction.Run();
    }

    private void FixedUpdate()
    {
        if (!isPicked) currentAction.FixedRun();
    }

    private void SetAction(TurretAction action)
    {
        currentAction = Instantiate(action);
        currentAction.turret = this;
        currentAction.player = player;
        currentAction.Init();
    }

    public void DestroyTurret()
    {
        foreach (var item in currentAction.DroppedItems)
        {
            inventory.AddItem(item.item, item.amount);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        action.TriggerEnter(collision, this);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        action.TriggerStay(collision, this);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        action.TriggerExit(collision, this);
    }
}
