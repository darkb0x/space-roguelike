using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Oven.Manager
{
    using Player.Inventory;

    [CreateAssetMenu(fileName = "Oven craft list", menuName = "Game/new Oven craft list")]
    public class OvenCraftList : ScriptableObject
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
