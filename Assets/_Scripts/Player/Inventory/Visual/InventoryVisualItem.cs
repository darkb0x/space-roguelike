using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Player.Inventory.Visual
{
    public class InventoryVisualItem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image ItemIconImage;
        [SerializeField] private TextMeshProUGUI ItemAmountText;

        public InventoryItem Item { get; private set; }

        public void UpdateVisual(InventoryItem item, int amount)
        {
            Item = item;

            ItemIconImage.sprite = Item.Icon;
            ItemAmountText.text = amount.ToString();
        }
    }
}
