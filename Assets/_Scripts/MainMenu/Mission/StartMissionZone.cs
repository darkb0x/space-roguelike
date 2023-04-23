using UnityEngine;

namespace Game.MainMenu.Mission
{
    using Player;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class StartMissionZone : MonoBehaviour
    {
        [SerializeField] private PlayerInteractObject InteractComponent;

        private MissionChooseManager MissionChooseManager;

        private void Start()
        {
            MissionChooseManager = Singleton.Get<MissionChooseManager>();

            InteractComponent.OnPlayerEnter += PlayerEnter;
            InteractComponent.OnPlayerExit += PlayerExit;
        }

        private void PlayerEnter(Collider2D coll)
        {
            MissionChooseManager.StartMissionTimer();
        }
        private void PlayerExit(Collider2D coll)
        {
            MissionChooseManager.StopMissionTimer();
        }

        private void OnDisable()
        {
            InteractComponent.OnPlayerEnter -= PlayerEnter;
            InteractComponent.OnPlayerExit -= PlayerExit;
        }
    }
}
