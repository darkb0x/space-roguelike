using UnityEngine;

namespace Game.CraftSystem
{
    using Player;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class LearnWorkbanch : MonoBehaviour
    {
        private LearnCSManager LearnCSManager;
        public void OpenMenu()
        {
            if(LearnCSManager == null)
            {
                LearnCSManager = Singleton.Get<LearnCSManager>();
            }

            LearnCSManager.OpenMenu();
        }
    }
}
