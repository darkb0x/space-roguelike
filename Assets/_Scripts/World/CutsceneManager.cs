using UnityEngine;

namespace Game
{
    using Player;

    public class CutsceneManager : MonoBehaviour, IService, IEntryComponent<PlayerController>
    {
        private PlayerController _player;

        public void Initialize(PlayerController player)
        {
            _player = player;
        }

        public void StopPlayerMove(Transform posTransform)
        {
            StopPlayerMove();
            _player.transform.position = posTransform.position;
        }
        public void StopPlayerMove()
        {
            _player.SetState(_player.StandingState);
        }
        public void ContinuePlayerMove()
        {
            _player.SetState(_player.DefaultState);
        }

        public void LockPlayerPosition(Transform posPosition)
        {
            StopPlayerMove();
            _player.transform.SetParent(posPosition);
            _player.transform.localPosition = Vector2.zero;
            _player.GetCollider().enabled = false;
            _player.SetComponentEnabled(_player.Health, false);
        }
        public void UnlockPlayerPosition()
        {
            transform.SetParent(null);
            ContinuePlayerMove();
            _player.GetCollider().enabled = false;
            _player.SetComponentEnabled(_player.Health, false);
        }
    }
}