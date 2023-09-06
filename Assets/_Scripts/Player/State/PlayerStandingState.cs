namespace Game.Player.State
{
    public class PlayerStandingState : PlayerState
    {
        public override void Enter()
        {
            _player.SetComponentEnabled(_player.Movement, false);
        }
    }
}
