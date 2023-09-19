using System;
using System.Collections;
using UnityEngine;

namespace Game.Player.Components
{
    public class HealthController : PlayerComponent
    {
        public readonly float MaxHealth;
        public readonly float InvulnerabilityTime;

        public Action<int> OnHealthChanged;
        public float Health { get { return _health; } }

        private float _health;

        public HealthController(ComponentConfig componentConfig, HealthConfig config) : base(componentConfig)
        {
            if(config.MaxHealth <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(config.MaxHealth));
            if(config.InvulnerabilityTime < 0)
                throw new System.ArgumentOutOfRangeException(nameof(config.InvulnerabilityTime));

            MaxHealth = config.MaxHealth;
            InvulnerabilityTime = config.InvulnerabilityTime;
            _health = MaxHealth;
        }

        public void TakeDamage(float value)
        {
            if (!Enabled)
                return;

            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            _health = Mathf.Clamp(_health - value, 0, MaxHealth);
            Debug.Log($"Player got damage({value}), {_health}/{MaxHealth}");

            if (_health == 0)
            {
                Die();
            }
            else
            {
                _player.StartCoroutine(_visual.PlayerHurt(InvulnerabilityTime));
                _player.StartCoroutine(InvulnerabilityCoroutine());
            }

            OnHealthChanged?.Invoke((int)_health);
        }
        public void Die()
        {
            if (!Enabled)
                return;

            _player.SetState(_player.DeadState);
            _visual.PlayerDead();

            Debug.Log("Player died");
        }

        private IEnumerator InvulnerabilityCoroutine()
        {
            _player.SetState(_player.InvulnerabityState);
            yield return new WaitForSeconds(InvulnerabilityTime);
            _player.SetState(_player.DefaultState);
        }
    }
}
