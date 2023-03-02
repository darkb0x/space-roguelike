using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Drill
{
    using Player.Inventory;

    public class DrillInventoryVisual : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image ItemImage;
        [SerializeField] private TextMeshProUGUI ItemAmountText;
        [SerializeField] private GameObject InventoryVisual;

        public void UpdateVisual(InventoryItem item, int amount)
        {
            if(item != null)
            {
                ItemImage.sprite = item.LowSizeIcon;
                ItemAmountText.text = amount.ToString();
            }
            else
            {
                EnableVisual(false);
            }
        }

        public void EnableVisual(bool enabled)
        {
            InventoryVisual.SetActive(enabled);
        }
    }
}
