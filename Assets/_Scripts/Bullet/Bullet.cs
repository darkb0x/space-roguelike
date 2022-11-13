using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Bullets
{
    using Enemy;

    public class Bullet : MonoBehaviour
    {
        [Header("скорость, дистанция, время")]
        public float speed;
        public float lifeTime;
        public float distance;

        [Header("урон")]
        [NaughtyAttributes.ReadOnly] public float damage;
        public LayerMask whatIsSolid;
        public GameObject destroyEffect;

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
                if (hitInfo.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(damage);
                }
                DestroyBullet();
            }
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }

        public void DestroyBullet()
        {
            if (destroyEffect != null)
                Instantiate(destroyEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
