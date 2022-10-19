using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Bullet
{
    new Transform transform;

    public override void Init(int dmg)
    {
        transform = GetComponent<Transform>();

        damage = dmg;
        Destroy(gameObject, lifeTime);
    }

    public override void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsSolid);
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
            {
                enemy.TakeDamage(damage);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
