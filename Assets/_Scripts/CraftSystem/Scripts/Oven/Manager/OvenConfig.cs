using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Oven.Manager
{
    using Player.Inventory;

    [CreateAssetMenu(fileName = "Oven Config", menuName = "Game/new Oven config")]
    public class OvenConfig : ScriptableObject
    {
        [System.Serializable]
        public class craft
        {
            public string name;

            [System.Serializable]
            public struct s_item
            {
                public InventoryItem item;
                [Min(1)] public int amount;
            }

            [Space]
            public List<s_item> firstItems = new List<s_item>(1);
            public s_item finalItem = new s_item() { amount = 1 };
        }

        [SerializeField] private List<craft> m_Items = new List<craft>();

        public List<craft> Items => m_Items;
    }
}
