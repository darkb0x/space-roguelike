namespace Game.Player.State
{
    public class PlayerDefaultState : PlayerState
    {
        public override void Enter()
        {
            _player.EnableAllComponents();
        }
    }
}
