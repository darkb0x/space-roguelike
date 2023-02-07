using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    using Player;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class LearnWorkbanch : MonoBehaviour, IDamagable
    {
        LearnCSManager learnSystem;

        [SerializeField] private Enemy.EnemyTarget EnemyTarget;

        private void Start()
        {
            learnSystem = FindObjectOfType<LearnCSManager>();

            EnemyTarget.Initialize(this);
        }

        public void OpenMenu()
        {
            learnSystem.OpenMenu();
        }

        void IDamagable.Die()
        {
            Destroy(gameObject);
        }
    }
}
