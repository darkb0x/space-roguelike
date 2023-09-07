using UnityEngine;
using TMPro;
using Game.Save;

namespace Game.Inventory
{
    public class MoneyFieldVisual : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Text;

        private Inventory _inventory;

        private void Start()
        {
            _inventory = ServiceLocator.GetService<Inventory>();
            UpdateField(SaveManager.SessionSaveData.Money);
        }
        private void OnEnable()
        {
            if(_inventory)
                _inventory.OnMoneyChanged += UpdateField;
        }
        private void OnDisable()
        {
            if(_inventory)
                _inventory.OnMoneyChanged -= UpdateField;
        }

        protected virtual void UpdateField(int value)
        {
            Text.text = value + "$";
        }
    }
}