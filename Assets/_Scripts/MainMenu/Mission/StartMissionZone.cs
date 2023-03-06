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
            InteractComponent.OnPlayerEnter += x => MissionChooseManager.Instance.StartMissionTimer();
            InteractComponent.OnPlayerExit += x => MissionChooseManager.Instance.StopMissionTimer();
        }
    }
}
