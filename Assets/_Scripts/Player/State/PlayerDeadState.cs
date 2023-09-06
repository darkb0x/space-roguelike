namespace Game.Player.State
{
    public class PlayerDeadState : PlayerState
    {
        public override void Enter()
        {
            _player.StopAllCoroutines();
            _player.DisableAllComponents();
        }
    }
}
