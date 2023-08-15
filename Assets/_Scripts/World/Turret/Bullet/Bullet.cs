using UnityEngine;

namespace Game.Turret.Bullets
{
    using Enemy;

    public class Bullet : MonoBehaviour
    {
        public float speed;
        public float lifeTime;
        public float distance;

        [NaughtyAttributes.ReadOnly] public float damage;
        public LayerMask whatIsSolid;
        public GameObject destroyEffect;

        public virtual void Init(float dmg)
        {
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
