using UnityEngine;

namespace Game.MainMenu.MissionChoose
{
    using Player;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class StartMissionZone : MonoBehaviour
    {
        [SerializeField] private PlayerInteractObject InteractComponent;
        [SerializeField] private GameObject ZoneVisual;

        private MissionChooseManager MissionChooseManager;

        private void Start()
        {
            MissionChooseManager = Singleton.Get<MissionChooseManager>();

            ZoneVisual.SetActive(false);

            InteractComponent.OnPlayerEnter += PlayerEnter;
            InteractComponent.OnPlayerExit += PlayerExit;
            MissionChooseManager.OnMissionSelected += OnPlayerChoosedMission;
        }

        private void OnDisable()
        {
            InteractComponent.OnPlayerEnter -= PlayerEnter;
            InteractComponent.OnPlayerExit -= PlayerExit;
            MissionChooseManager.OnMissionSelected -= OnPlayerChoosedMission;
        }

        private void OnPlayerChoosedMission(Planet.PlanetSO mission)
        {
            if(mission != null)
            {
                ZoneVisual.SetActive(true);
            }
            else
            {
                ZoneVisual.SetActive(false);
            }
        }

        private void PlayerEnter(Collider2D coll)
        {
            MissionChooseManager.StartMissionTimer();
        }
        private void PlayerExit(Collider2D coll)
        {
            MissionChooseManager.StopMissionTimer();
        }

    }
}
