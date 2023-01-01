using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Inventory
{
    public class InventoryScreen : MonoBehaviour
    {
        [SerializeField] private InventoryScreen_element elementPrefab;
        private List<InventoryScreen_element> elements = new List<InventoryScreen_element>();

        public void UpdateData(List<PlayerInventory.Item> list)
        {
            if(elements.Count == 0 || elements == null)
            {
                foreach (var item in list)
                {
                    InventoryScreen_element obj = Instantiate(elementPrefab, transform).GetComponent<InventoryScreen_element>();
                    elements.Add(obj);
                    obj.UpdateData(item);
                }
            }

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].UpdateData(list[i]);
            }
        }
    }
}
