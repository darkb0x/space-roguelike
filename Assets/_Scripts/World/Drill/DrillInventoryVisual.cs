using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Drill
{
    using Inventory;

    public class DrillInventoryVisual : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image ItemImage;
        [SerializeField] private TextMeshProUGUI ItemAmountText;
        [SerializeField] private GameObject InventoryVisual;

        public void UpdateVisual(ItemData item)
        {
            if(item != null && item.Item != null)
            {
                ItemImage.sprite = item.Item.LowSizeIcon;
                ItemAmountText.text = item.Amount.ToString();
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
