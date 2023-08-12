using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.MainMenu.MissionChoose.Planet.Visual;

namespace Game.CraftSystem.Visual.Node.Components
{
    public class NodeItemFieldVisual : MonoBehaviour
    {
        [SerializeField] private Image ItemIconImage;
        [SerializeField] private TextMeshProUGUI ItemAmountText;

        private bool _hidded;
        private int _itemAmount;

        public void UpdateVisual(Sprite itemIcon, int itemAmount)
        {
            _itemAmount = itemAmount;
            ItemIconImage.sprite = itemIcon; 

            if (_hidded)
                SetHideStyle();
            else
                SetDefaultStyle();
        }

        public void SetDefaultStyle()
        {
            _hidded = false;

            ItemIconImage.color = Color.white;
            ItemAmountText.text = _itemAmount.ToString();
        }
        public void SetHideStyle()
        {
            _hidded = true;

            ItemIconImage.color = Color.black;
            ItemAmountText.text = "???";
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }
        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
