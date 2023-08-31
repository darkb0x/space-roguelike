﻿using UnityEngine;

namespace Game.CraftSystem.Research
{
    public class ResearchTable : MonoBehaviour
    {
        private ResearchManager ResearchManager;

        private void Start()
        {
            ResearchManager = ServiceLocator.GetService<ResearchManager>();
        }

        public void OpenResearchManager()
        {
            ResearchManager.Open();
        }
    }
}