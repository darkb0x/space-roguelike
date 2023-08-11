﻿using UnityEngine;

namespace Game.CraftSystem
{
    public class ResearchTable : MonoBehaviour
    {
        private ResearchManager ResearchManager;

        private void Start()
        {
            ResearchManager = Singleton.Get<ResearchManager>();
        }

        public void OpenResearchManager()
        {
            ResearchManager.Open();
        }
    }
}