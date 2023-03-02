using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Player
{
    using Inventory;

    public class RepairObjectItemUIVisual : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image ItemImage;
        [SerializeField] private TextMeshProUGUI ItemAmountText;

        public InventoryItem itemData { get; private set; }

        public void UpdateVisual(InventoryItem item, int itemAmount, Color iconColor)
        {
            itemData = item;

            ItemImage.sprite = itemData.LowSizeIcon;
            ItemAmountText.text = itemAmount.ToString();
            ItemAmountText.color = iconColor;
        }
    }
}
