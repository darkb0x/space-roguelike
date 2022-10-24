using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TurretAI : MonoBehaviour
{
    [System.Serializable]
    public struct obj
    {
        public string name;
        public GameObject gameObj;
    }

    [Header("parameters")]
    public Transform shotPos;
    public Transform turret_canon;
    [SerializeField] private float time_smooth = 1.2f;
    [Expandable] public TurretStats stats;

    [HideInInspector] public GameObject bulletPrefab;
    [HideInInspector] public float damage;
    [HideInInspector] public float timeBtwAttack = 0.3f;
    [HideInInspector] public float recoil = 0f;

    [Header("actions")]
    [SerializeField, Expandable] private TurretAction action;
    [ReadOnly] public GameObject currentEnemy;
    [ReadOnly] public bool enemyInZone;
    [Tag] public string enemyTag;

    [Header("other")]
    public bool isPicked;
    [SerializeField] private Collider2D coll;
    [SerializeField, ReadOnly] List<GameObject> targets = new List<GameObject>();

    [Space(10)]
    [SerializeField] private List<obj> additionalObjects = new List<obj>();

    TurretAction currentAction;
    PlayerController player;
    PlayerInventory inventory;
    new Transform transform;

    private void OnDrawGizmosSelected()
    {
        if(currentEnemy != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentEnemy.transform.position);
        }
    }

    #region additional object | functions
    public GameObject _GetObject(string objName)
    {
        foreach (var item in additionalObjects)
        {
            if(item.name == objName)
            {
                return item.gameObj;
            }
        }
        Debug.LogError($"Turret AI | GetObject() | {objName} in list additionalObjects, not valid!");
        return null;
    }
    public void _AddObject(GameObject gameObj, string objName)
    {
        obj o = new obj();
        o.name = objName;
        o.gameObj = gameObj;
        additionalObjects.Add(o);
    }
    public void _RemoveObject(string objName)
    {
        int index = 0;
        for (int i = 0; i < additionalObjects.Count; i++)
        {
            if(additionalObjects[i].name == objName)
            {
                index = i;
                break;
            }
        }
        additionalObjects.RemoveAt(index);
    }
    #endregion

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        inventory = FindObjectOfType<PlayerInventory>();
        transform = GetComponent<Transform>();

        SetAction(action);

        bulletPrefab = stats.bulletPrefab;
        damage = stats.damage;
        timeBtwAttack = stats.timeBtwAttack;
        recoil = stats.recoil;

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
        if (currentEnemy != null)
        {
            RotateToTarget(currentEnemy);
        }
        enemyInZone = (targets.Count > 0);

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
            if (!targets.Contains(collision.gameObject))
                targets.Add(collision.gameObject);
        }

        action.TriggerEnter(collision, this);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == enemyTag)
        {
            if (currentEnemy == null)
            {
                currentEnemy = GetNearestEnemy();
            }
        }

        action.TriggerStay(collision, this);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == enemyTag)
        {
            targets.Remove(collision.gameObject);
            if (currentEnemy == collision.gameObject)
            {
                currentEnemy = GetNearestEnemy();
            }
        }

        action.TriggerExit(collision, this);
    }

    private GameObject GetNearestEnemy()
    {
        try
        {
            GameObject enemy = targets[0];
            float curDistance = Vector2.Distance(transform.position, enemy.transform.position);
            for (int i = 1; i < targets.Count; i++)
            {
                if (Vector2.Distance(transform.position, targets[i].transform.position) < curDistance)
                    enemy = targets[i];
            }
            return enemy;
        }
        catch (System.Exception)
        {
            return null;
        }
    }
}
