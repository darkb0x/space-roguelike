using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float frontVisionDistance;
    [SerializeField] private float sideVisionDistance;
    [SerializeField] private float sideVisionFactor = 0.8f;
    [Space]
    [SerializeField] private GameObject exploisonGameObj;
    [Space]
    [SerializeField] private LayerMask whatIsSolid;
    [SerializeField] private float radius;
    [NaughtyAttributes.ReadOnly] public float damage;
    [Space]
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float rotationTime = 1.2f;

    bool selfMove = false;

    new Transform myTansform;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        Debug.DrawRay(transform.position, transform.right * frontVisionDistance, Color.blue);
        Debug.DrawRay(transform.position, (transform.right + transform.up * sideVisionDistance) * frontVisionDistance * sideVisionFactor, Color.blue);
        Debug.DrawRay(transform.position, (transform.right + transform.up * -sideVisionDistance) * frontVisionDistance * sideVisionFactor, Color.blue);
    }

    private void Update()
    {
        if(selfMove)
        {
            myTansform.Translate(Vector2.right * speed * Time.deltaTime);

            if(lifeTime <= 0)
            {
                SpawnExploinson();
            }
            else
            {
                lifeTime -= Time.deltaTime;
            }
        }
    }

    public void Init(float dmg)
    {
        myTansform = transform;

        damage = dmg;
    }

    public void ControlMoveToTarget(Vector3 target)
    {
        myTansform.Translate(Vector2.right * speed * Time.deltaTime);

        Vector3 difference = target - myTansform.position;
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        myTansform.rotation = Quaternion.Lerp(myTansform.rotation, Quaternion.Euler(0, 0, rotation_z), rotationTime * Time.deltaTime);

        if (EnemyInVision())
        {
            SpawnExploinson();
        }
    }

    public void StopControl()
    {
        selfMove = true;
    }

    bool EnemyInVision()
    {
        RaycastHit2D hit_left = Physics2D.Raycast(myTansform.position, myTansform.right, frontVisionDistance, whatIsSolid);
        RaycastHit2D hit_middle = Physics2D.Raycast(myTansform.position, myTansform.right + myTansform.up * sideVisionDistance, frontVisionDistance * sideVisionFactor, whatIsSolid);
        RaycastHit2D hit_right = Physics2D.Raycast(myTansform.position, myTansform.right + myTansform.up * -sideVisionDistance, frontVisionDistance * sideVisionFactor, whatIsSolid);

        if (hit_left.collider != null)
            return true;
        if (hit_middle.collider != null)
            return true;
        if (hit_right.collider != null)
            return true;

        return false;
    }

    void SpawnExploinson()
    {
        Exploison obj = Instantiate(exploisonGameObj, myTansform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))).GetComponent<Exploison>();
        obj.Init(damage, whatIsSolid, radius);

        Destroy(gameObject);
    }
}
