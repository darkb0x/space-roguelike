using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game.CraftSystem.Visual.Category
{
    public class CraftCategoryController : MonoBehaviour
    {
        [SerializeField] private CategoryButton CategoryButtonPrefab;
        [SerializeField] private Transform ButtonsParent;

        public void Initialize(Dictionary<ResearchTree, Action> categories)
        {
            foreach (var researchTree in categories.Keys)
            {
                var button = Instantiate(CategoryButtonPrefab, ButtonsParent);
                button.Intialize(researchTree.Icon, researchTree.Title, categories[researchTree]);
            }
        }
    }
}