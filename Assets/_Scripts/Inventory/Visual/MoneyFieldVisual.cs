using UnityEngine;
using TMPro;

namespace Game.Inventory
{
    public class MoneyFieldVisual : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Text;

        private Inventory _inventory;

        private void Start()
        {
            _inventory = ServiceLocator.GetService<Inventory>();
            _inventory.OnMoneyChanged += UpdateField;
            UpdateField(_inventory.Money);
        }
        private void OnDisable()
        {
            _inventory.OnMoneyChanged -= UpdateField;
        }

        protected virtual void UpdateField(int value)
        {
            Text.text = value + "$";
        }
    }
}