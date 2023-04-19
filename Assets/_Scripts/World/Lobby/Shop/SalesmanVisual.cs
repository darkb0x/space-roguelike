using UnityEngine;

namespace Game.Lobby.Shop
{
    public class SalesmanVisual : MonoBehaviour
    {
        [SerializeField] private Animator Anim;
        [SerializeField, NaughtyAttributes.AnimatorParam("Anim")] private string Anim_playerNearBool;
        [Space]
        [SerializeField] private float NearDistance = 4f;

        private Transform playerTransform;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, NearDistance);
        }

        private void Start()
        {
            playerTransform = FindObjectOfType<Player.PlayerController>().transform;
        }

        private void Update()
        {
            bool playerNear = Vector2.Distance(transform.position, playerTransform.position) <= NearDistance;
            Anim.SetBool(Anim_playerNearBool, playerNear);
        }
    }
}
