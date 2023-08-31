using UnityEngine;

namespace Game.Player.Components
{
    public class OxygenController : PlayerComponent, IRequireUpdate
    {
        public readonly float MaxOxygen;
        public readonly float LowOxygenValue;
        public readonly float OxygenUseSpeed;

        public float Oxygen { get { return _oxygen; } }

        private float _oxygen;

        public OxygenController(ComponentConfig componentConfig, OxygenConfig config) : base(componentConfig)
        { 
            if (config.MaxOxygen <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(config.MaxOxygen));
            if (config.StartOxygenValue <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(config.StartOxygenValue));
            if (config.LowOxygenValue <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(config.LowOxygenValue));
            if (config.OxygenUseSpeed <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(config.OxygenUseSpeed));

            MaxOxygen = config.MaxOxygen;
            LowOxygenValue = config.LowOxygenValue;
            OxygenUseSpeed = config.OxygenUseSpeed;

            _oxygen = config.StartOxygenValue;
            
            _visual.EnableOxygenVisual(true);
        }

        public override void Disable()
        {
            base.Disable();

            AddOxygen(MaxOxygen);
        }

        public void Update()
        {
            if (!Enabled)
                return;

            HandleOxygen();
        }

        private void HandleOxygen()
        {
            if (_oxygen == 0)
            {
                _player.Health.Die();
                return;
            }

            bool isLowOxygen = _oxygen <= LowOxygenValue;
            _visual.PlayerLowOxygen(isLowOxygen, _oxygen, LowOxygenValue);

            _oxygen = Mathf.Clamp(_oxygen - (Time.deltaTime * OxygenUseSpeed), 0, MaxOxygen);  
            _visual.UpdateOxygenVisual(_oxygen, MaxOxygen);
        }

        public bool AddOxygen(float value)
        {   
            if (_oxygen == MaxOxygen)
                return false;

            _oxygen = Mathf.Clamp(_oxygen + value, 0, MaxOxygen);
            _visual.UpdateOxygenVisual(_oxygen, MaxOxygen);

            return true;
        }
    }
}
