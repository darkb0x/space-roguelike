namespace Game.Player.State
{
    public class PlayerDefaultState : PlayerState
    {
        public override void Enable()
        {
            _player.EnableAllComponents();
        }
    }
}
