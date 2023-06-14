using UnityEngine;

namespace Game.MainMenu.MissionChoose
{
    using Player;

    [RequireComponent(typeof(PlayerInteractObject))]
    public class StartMissionZone : MonoBehaviour
    {
        [SerializeField] private PlayerInteractObject InteractComponent;
        [SerializeField] private GameObject ZoneVisual;
        [SerializeField] private Transform ZoneFill;
        [SerializeField, Range(0, 1), NaughtyAttributes.OnValueChanged("CalculateFill")] private float Fill;

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

        private void Update()
        {
            if(ZoneVisual.activeSelf)
            {
                Fill = Mathf.Abs(MissionChooseManager.startMissionTimer / MissionChooseManager.m_StartMissionTimer);
                CalculateFill();
            }
        }

        private void CalculateFill()
        {
            ZoneFill.localScale = new Vector3(1, Fill, 1);

            Vector3 fillPos = new Vector3()
            {
                x = ZoneFill.localPosition.x,
                y = -0.5f - (Fill * -0.5f),
                z = ZoneFill.localPosition.z
            };

            ZoneFill.localPosition = fillPos;
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
