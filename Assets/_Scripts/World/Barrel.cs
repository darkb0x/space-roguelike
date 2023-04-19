using UnityEngine;

namespace Game
{
    using Player;

    public class Barrel : MonoBehaviour
    {
        [SerializeField] private float force;
        [SerializeField] private float torqueForce;
        [Space]
        [SerializeField, NaughtyAttributes.Tag] private string playerTag;

        [NaughtyAttributes.ShowNonSerializedField] bool playerInZone = false;

        Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Push()
        {
            rb.AddForce(new Vector2(Random.Range(-force, force), Random.Range(-force, force)));
            rb.AddTorque(Random.Range(Random.Range(-1, 1) * torqueForce, Random.Range(-1, 1) * torqueForce));
        }

        public void Update()
        {
            if(playerInZone)
            {
                if(Input.GetKeyDown(KeyCode.E))
                {
                    Push();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent<PlayerController>(out PlayerController p))
            {
                playerInZone = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            playerInZone = false;
        }
    }
}
