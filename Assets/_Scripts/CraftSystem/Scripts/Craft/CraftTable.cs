using System.Collections;
using UnityEngine;

namespace Game.CraftSystem.Craft
{
    public class CraftTable : MonoBehaviour
    {
        private CraftManager CraftManager;

        private void Awake()
        {
            CraftManager = ServiceLocator.GetService<CraftManager>();
        }

        public void OpenCraftPanel()
        {
            CraftManager.Open(this);
        }
    }
}