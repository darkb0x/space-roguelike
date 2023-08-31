using UnityEngine;

namespace Game.Session
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private GameObject Visual;
        [SerializeField] private Animator VisualAnim;
        [SerializeField] private Transform PlayerParent;

        private void Start()
        {
            Visual.SetActive(false);
            VisualAnim.enabled = false;
        }

        public void Activate()
        {
            Visual.SetActive(true);
            VisualAnim.enabled = true;

            var sessionManager = ServiceLocator.GetService<SessionManager>();
            sessionManager.StartLoadingLobby();
            sessionManager.SetPlayerIntoRocket(PlayerParent);
        }
    }
}
