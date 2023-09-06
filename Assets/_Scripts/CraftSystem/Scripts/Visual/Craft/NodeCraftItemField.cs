using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.CraftSystem
{
    using Inventory;

    public class NodeCraftItemField : MonoBehaviour
    {
        [SerializeField] private Image ItemImage;
        [SerializeField] private TextMeshProUGUI ItemAmountText;

        public void Initialize(ItemData data)
        {
            ItemImage.sprite = data.Item.LowSizeIcon;
            ItemAmountText.text = data.Amount.ToString();
        }

        public void SetEnoughStyle()
        {
            ItemAmountText.color = Color.white;
        }
        public void SetNotEnoughStyle()
        {
            ItemAmountText.color = Color.red;
        }
    }
}
