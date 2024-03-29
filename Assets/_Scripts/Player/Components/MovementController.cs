﻿using Game.Input;
using UnityEngine;

namespace Game.Player.Components
{
    using UI;

    public class MovementController : PlayerComponent, IRequireUpdate, IRequireFixedUpdate
    {
        public readonly float Speed;

        private Rigidbody2D _rigidbody;
        private UIWindowService _uiWindowService;

        private Vector2 _moveInput;
        private Vector2 _lookDirection;

        public MovementController(ComponentConfig componentConfig, MovementConfig config) : base(componentConfig)
        {
            if (config.Speed < 0)
                throw new System.ArgumentOutOfRangeException(nameof(config.Speed));

            Speed = config.Speed;

            _rigidbody = _player.GetComponent<Rigidbody2D>();

            _uiWindowService = ServiceLocator.GetService<UIWindowService>();
        }

        public void Update()
        {
            if (!Enabled)
                return;

            _moveInput = _input.GetMoveValue();

            if (_moveInput.sqrMagnitude > Mathf.Epsilon)
            {
                _lookDirection = _moveInput; 
                _visual.PlayerRunAnimation();
            }
            else
            {
                _lookDirection = -(_rigidbody.position - InputManager.Instance.GetWorldMousePosition()).normalized;
                _visual.PlayerIdleAnimation();
            }

            if(!_uiWindowService.IsAnyWindowOpened())
                _visual.PlayerLookDirection(_lookDirection);
        }

        public void FixedUpdate()
        {
            if (!Enabled)
                return;

            _rigidbody.MovePosition(_rigidbody.position + (_moveInput * Speed * Time.fixedDeltaTime));
        }

        public Vector2 GetMoveDirection()
            => _moveInput;
    }
}
