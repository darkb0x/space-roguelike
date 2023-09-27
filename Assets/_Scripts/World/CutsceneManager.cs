using UnityEngine;
using UnityEngine.Playables;

namespace Game
{
    using Game.UI.HUD;
    using Player;

    public class CutsceneManager : MonoBehaviour, IService, IEntryComponent<PlayerController, CameraController, HUDService>
    {
        [SerializeField] private PlayableDirector StartCutscene;

        private PlayerController _player;
        private CameraController _camera;
        private HUDService _hudService;

        public void Initialize(PlayerController player, CameraController camera, HUDService hudService)
        {
            _player = player;
            _camera = camera;
            _hudService = hudService;

            StartCutscene.Play();

            Debug.Log("Cutscene action: Initialize. Playing start cutscene...");
        }

        public void StopPlayerMove(Transform posTransform)
        {
            StopPlayerMove();
            _player.transform.position = posTransform.position;

            Debug.Log($"Cutscene action: StopPlayerMove(Transfrom->{posTransform.name})");
        }
        public void StopPlayerMove()
        {
            _player.SetState(_player.StandingState);

            Debug.Log("Cutscene action: StopPlayerMove()");
        }
        public void ContinuePlayerMove()
        {
            _player.SetState(_player.DefaultState);

            Debug.Log("Cutscene action: ContinuePlayerMove()");
        }

        public void LockPlayerPosition(Transform posPosition)
        {
            StopPlayerMove();
            _player.transform.SetParent(posPosition);
            _player.transform.localPosition = Vector2.zero;
            _player.GetCollider().enabled = false;
            _player.SetComponentEnabled(_player.Health, false);

            Debug.Log($"Cutscene action: LockPlayerPosition(Transform->{posPosition.name})");
        }
        public void UnlockPlayerPosition()
        {
            _player.transform.SetParent(null);
            _player.GetCollider().enabled = true;
            _player.SetComponentEnabled(_player.Health, true);
            ContinuePlayerMove();

            Debug.Log("Cutscene action: UnlockPlayerPosition()");
        }

        public void LockCamera()
        {
            _camera.Lock(true);
        }
        public void UnlockCamera()
        {
            _camera.Lock(false);
        }

        public void ChangeHudEnabled(bool enabled)
        {
            _hudService.RootCanvas.gameObject.SetActive(enabled); 
        }

        public void SpawnGameObjectAtPlayerPosition(GameObject obj)
        {
            Instantiate(obj, _player.transform.position, Quaternion.identity);
        }
    }
}