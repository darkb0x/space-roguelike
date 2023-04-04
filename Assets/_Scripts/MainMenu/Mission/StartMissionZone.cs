using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MainMenu.Mission
{
    using Player;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class StartMissionZone : MonoBehaviour
    {
        [SerializeField] private PlayerInteractObject InteractComponent;

        private void Start()
        {
            InteractComponent.OnPlayerEnter += PlayerEnter;
            InteractComponent.OnPlayerExit += PlayerExit;
        }

        private void PlayerEnter(Collider2D coll)
        {
            MissionChooseManager.Instance.StartMissionTimer();
        }
        private void PlayerExit(Collider2D coll)
        {
            MissionChooseManager.Instance.StopMissionTimer();
        }

        private void OnDisable()
        {
            InteractComponent.OnPlayerEnter -= PlayerEnter;
            InteractComponent.OnPlayerExit -= PlayerExit;
        }
    }
}
