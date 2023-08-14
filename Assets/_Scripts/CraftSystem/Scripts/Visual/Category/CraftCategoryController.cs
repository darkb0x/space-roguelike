using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game.CraftSystem.Visual.Category
{
    using Research;
    using Craft;

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
        public void Initialize(Dictionary<CraftContainer, Action> categories)
        {
            foreach (var craftContainer in categories.Keys)
            {
                var button = Instantiate(CategoryButtonPrefab, ButtonsParent);
                button.Intialize(craftContainer.Icon, craftContainer.Title, categories[craftContainer]);

                if (craftContainer.GetCrafts().Length == 0)
                    button.SetEnabled(false);
                else
                    button.SetEnabled(true);
            }
        }
    }
}