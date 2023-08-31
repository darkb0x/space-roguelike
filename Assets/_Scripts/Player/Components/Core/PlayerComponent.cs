namespace Game.Player.Components
{
    using Input;
    using System;

    public abstract class PlayerComponent
    {
        protected readonly PlayerController _player;
        protected readonly PlayerVisual _visual;
        protected readonly PlayerInputHandler _input;

        public Action OnEnableChanged;
        public bool Enabled { get { return _enabled; } }

        private bool _enabled;

        public PlayerComponent(ComponentConfig componentConfig)
        {
            _player = componentConfig.Player;
            _visual = componentConfig.Visual;
            _input = componentConfig.Input;
        }

        public virtual void Enable() 
        {
            _enabled = true;
            OnEnableChanged?.Invoke();
        }
        public virtual void Disable() 
        {
            _enabled = false;
            OnEnableChanged?.Invoke();
        }
    }
}
