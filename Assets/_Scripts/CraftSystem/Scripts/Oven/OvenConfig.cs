using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Oven
{
    using Inventory;

    [CreateAssetMenu(fileName = "Oven Config", menuName = "Game/Configs/new Oven config")]
    public class OvenConfig : ScriptableObject
    {
        [System.Serializable]
        public class craft
        {
            public string name;

            [Space]
            public List<ItemData> firstItems = new List<ItemData>(1);
            public ItemData finalItem;
        }

        [SerializeField] private List<craft> m_Items = new List<craft>();

        public List<craft> Items => m_Items;
    }
}
