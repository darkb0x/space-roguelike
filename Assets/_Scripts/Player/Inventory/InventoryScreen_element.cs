using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Player.Inventory
{
    public class InventoryScreen_element : MonoBehaviour
    {
        [SerializeField] private Image UI_icon;
        [SerializeField] private TextMeshProUGUI UI_amount;

        public void UpdateData(PlayerInventory.Item pItem)
        {
            UI_icon.sprite = pItem.item.Icon;
            UI_amount.text = pItem.amount.ToString();
        }
    }
}
