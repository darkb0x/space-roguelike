using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("скорость, дистанция, время")]
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private float distance;

    [Header("урон")]
    [SerializeField, NaughtyAttributes.ReadOnly] public int damage;
    [SerializeField] private LayerMask whatIsSolid;

    new Transform transform;

    public void Init(int dmg)
    {
        transform = GetComponent<Transform>();

        damage = dmg;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
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
