using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player
{
    using UI.HUD;

    public class OxygenHUD : HUDElement
    {
        [SerializeField] private Image OxygenBarUI;

        public override HUDElementID ID => HUDElementID.Oxygen;

        private PlayerController _player;
        private float _maxOxygenValue;

        public override void Initialize()
        {
            _player = ServiceLocator.GetService<PlayerController>();

            _maxOxygenValue = _player.Oxygen.MaxOxygen;
            UpdateOxygenVisual(_maxOxygenValue);

            _player.Oxygen.OnOxygenChanged += UpdateOxygenVisual;
        }

        private void OnDestroy()
        {
            _player.Oxygen.OnOxygenChanged -= UpdateOxygenVisual;
        }

        private void UpdateOxygenVisual(float value)
        {
            OxygenBarUI.fillAmount = value / _maxOxygenValue;
        }
    }
}
