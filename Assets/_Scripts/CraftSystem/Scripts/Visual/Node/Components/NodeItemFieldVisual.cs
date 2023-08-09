using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.CraftSystem.Visual.Node.Components
{
    public class NodeItemFieldVisual : MonoBehaviour
    {
        [SerializeField] private Image ItemIconImage;
        [SerializeField] private TextMeshProUGUI ItemAmountText;

        public void UpdateVisual(Sprite itemIcon, int itemAmount)
        {
            ItemIconImage.sprite = itemIcon;
            ItemAmountText.text = itemAmount.ToString();
        }
    }
}
