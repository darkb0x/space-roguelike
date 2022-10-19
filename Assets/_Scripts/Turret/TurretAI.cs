using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TurretAI : MonoBehaviour
{
    [Header("parameters")]
    public Transform shotPos;
    public Transform turret_canon;
    [SerializeField] private float time_smooth = 1.2f;
    [Expandable] public TurretStats stats;

    [HideInInspector] public GameObject bulletPrefab;
    [HideInInspector] public int damage;
    [HideInInspector] public float timeBtwAttack = 0.3f;

    [Header("actions")]
    [SerializeField, Expandable] private TurretAction action;
    [ReadOnly] public GameObject currentEnemy;
    [ReadOnly] public bool enemyInZone;
    [Tag] public string enemyTag;

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

        bulletPrefab = stats.bulletPrefab;
        damage = stats.damage;
        timeBtwAttack = stats.timeBtwAttack;

        //coll.enabled = false;
    }

    public void Put()
    {
        isPicked = false;
        coll.enabled = true;
    }
    void RotateToTarget(GameObject target)
    {
        if (currentEnemy != null)
        {
            Vector3 dir = target.transform.position - turret_canon.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion a = turret_canon.rotation;
            Quaternion b = Quaternion.Euler(0, 0, angle);
            turret_canon.rotation = Quaternion.Lerp(a, b, time_smooth); //сдела сглаживание поворота ибо так красивее
        }
        else
        {
            return;
        }
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
        foreach (var item in stats.DroppedItems)
        {
            inventory.AddItem(item.item, item.amount);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == enemyTag)
        {
            currentEnemy = collision.gameObject;
        }

        action.TriggerEnter(collision, this);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == enemyTag)
        {
            enemyInZone = true;
            if (currentEnemy.GetComponent<EnemyAI>().hp <= 0)
            {
                currentEnemy = collision.gameObject;
            }
            if (currentEnemy != null)
            {
                RotateToTarget(currentEnemy);
            }
            else
            {
                currentEnemy = collision.gameObject;
            }
        }

        action.TriggerStay(collision, this);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == enemyTag)
        {
            enemyInZone = false;
        }

        action.TriggerExit(collision, this);
    }
}
