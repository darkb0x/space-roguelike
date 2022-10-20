using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGrenade : Bullet
{
    [Space]
    [SerializeField] private float speed_rotation;
    [SerializeField] private float radius;
    [Space]
    [SerializeField] private GameObject exploisonGameObj;

    Rigidbody2D rb;
    Transform myTransform;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public override void Init(float dmg)
    {
        rb = GetComponent<Rigidbody2D>();
        myTransform = GetComponent<Transform>();

        damage = dmg;

        rb.AddForce(myTransform.right * speed);

        StartCoroutine(explode());
    }

    public override void Update()
    {
        myTransform.Rotate(0, 0, speed_rotation);
        if (speed_rotation > 0)
            speed_rotation -= Time.deltaTime;
    }

    IEnumerator explode()
    {
        yield return new WaitForSeconds(lifeTime);
        SpawnExploinson();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<EnemyAI>(out EnemyAI enemy))
        {
            SpawnExploinson();
        }
    }

    void SpawnExploinson()
    {
        Exploison obj = Instantiate(exploisonGameObj, myTransform.position, Quaternion.Euler(0,0,Random.Range(0,360))).GetComponent<Exploison>();
        obj.Init(damage, whatIsSolid, radius);

        Destroy(gameObject);
    }
}
