using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    using Enemy;

    public class Exploison : MonoBehaviour
    {
        [SerializeField] private float radius;
        float damage;
        LayerMask layers;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public void Init(float dmg, LayerMask mask, float r)
        {
            damage = dmg;
            layers = mask;
            radius = r;

            Boom();
        }

        private void Boom()
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, radius, layers);

            foreach (var col in colls)
            {
                if (col.TryGetComponent<EnemyAI>(out EnemyAI enemy))
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        public void DestroyObj()
        {
            Destroy(gameObject);
        }
    }
}
