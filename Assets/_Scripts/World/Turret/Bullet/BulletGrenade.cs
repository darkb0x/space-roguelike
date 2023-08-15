using System.Collections;
using UnityEngine;

namespace Game.Turret.Bullets
{
    using Enemy;

    public class BulletGrenade : Bullet
    {
        [Space]
        [SerializeField] private float speed_rotation;
        [SerializeField] private float radius;
        [Space]
        [SerializeField] private GameObject exploisonGameObj;
        [Space]
        [SerializeField] private int exploisonAmount = 1;

        Rigidbody2D rb;
        Transform myTransform;
        private float factor;

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
            factor = speed_rotation / Mathf.PI;

            rb.AddForce(myTransform.right * speed);

            StartCoroutine(explode());
        }

        public override void Update()
        {
            myTransform.Rotate(0, 0, speed_rotation * Time.deltaTime);
            if (speed_rotation > 0)
                speed_rotation -= (factor * Time.deltaTime);
        }

        IEnumerator explode()
        {
            yield return new WaitForSeconds(lifeTime);
            SpawnExploinson();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<EnemyAI>(out EnemyAI enemy))
            {
                SpawnExploinson();
            }
        }

        void SpawnExploinson()
        {
            for (int i = 0; i < exploisonAmount; i++)
            {
                Exploison obj = Instantiate(exploisonGameObj, myTransform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))).GetComponent<Exploison>();
                float scaleValue = Random.Range(1, 2.5f);
                obj.transform.localScale = new Vector3(scaleValue, scaleValue, 1);
                obj.Init(damage, whatIsSolid, radius);
            }

            Destroy(gameObject);
        }
    }
}
