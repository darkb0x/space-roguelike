namespace Game.Player.State
{
    public class PlayerStandingState : PlayerState
    {
        public override void Enable()
        {
            _player.SetComponentEnabled(_player.Movement, false);
        }
    }
}
