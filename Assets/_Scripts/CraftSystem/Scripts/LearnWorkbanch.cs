using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    using Player;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class LearnWorkbanch : MonoBehaviour
    {
        LearnCSManager learnSystem;

        private void Start()
        {
            learnSystem = FindObjectOfType<LearnCSManager>();
        }

        public void OpenMenu()
        {
            learnSystem.OpenMenu();
        }
    }
}
