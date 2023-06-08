using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Inventory
{
    public interface IInventoryObserver
    {
        public void UpdateData(PlayerInventory inventory);
    }
}
