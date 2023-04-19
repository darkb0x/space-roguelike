using UnityEngine;

namespace Game.CraftSystem
{
    using Player;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class LearnWorkbanch : MonoBehaviour
    {
        public void OpenMenu()
        {
            LearnCSManager.Instance.OpenMenu();
        }
    }
}
