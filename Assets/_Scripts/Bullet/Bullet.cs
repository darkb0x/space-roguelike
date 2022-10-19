using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("��������, ���������, �����")]
    public float speed;
    public float lifeTime;
    public float distance;

    [Header("����")]
    [NaughtyAttributes.ReadOnly] public float damage;
    public LayerMask whatIsSolid;

    new Transform transform;

    public virtual void Init(float dmg)
    {
        transform = GetComponent<Transform>();

        damage = dmg;
        Destroy(gameObject, lifeTime);
    }

    public virtual void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsSolid);
        if (hitInfo.collider != null)
        {
            if(hitInfo.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
