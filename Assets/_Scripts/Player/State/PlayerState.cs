namespace Game.Player.State
{
    public abstract class PlayerState
    {
        protected PlayerController _player;

        public PlayerState Initialize(PlayerController player)
        {
            _player = player;

            return this;
        }

        public abstract void Enter();
    }
}
