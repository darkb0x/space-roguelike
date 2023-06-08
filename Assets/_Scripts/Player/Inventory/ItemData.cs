using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Inventory
{
    [System.Serializable]
    public class ItemData
    {
        [NaughtyAttributes.Expandable] public InventoryItem Item;
        public int Amount;

        public ItemData(InventoryItem item, int amount = 0)
        {
            Item = item;
            Amount = amount;
        }
    }
}
