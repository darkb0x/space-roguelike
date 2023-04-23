using UnityEngine;

namespace Game
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

            SessionManager sessionManager = Singleton.Get<SessionManager>();
            sessionManager.StartLoadingLobby();
            sessionManager.SetPlayerIntoRocket(PlayerParent);
        }
    }
}
