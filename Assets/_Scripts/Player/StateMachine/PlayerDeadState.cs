namespace Game.Player.State
{
    public class PlayerDeadState : PlayerState
    {
        public override void Enable()
        {
            _player.DisableAllComponents();
        }
    }
}
