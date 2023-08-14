using System.Collections;
using UnityEngine;

namespace Game.CraftSystem.Craft
{
    public class CraftTable : MonoBehaviour
    {
        private CraftManager CraftManager;

        private void Start()
        {
            CraftManager = Singleton.Get<CraftManager>();
        }

        public void OpenCraftPanel()
        {
            CraftManager.Open(this);
        }
    }
}