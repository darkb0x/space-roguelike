using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Game.MainMenu.Mission
{
    using Planet;
    using Player;

    public class MissionChooseManager : MonoBehaviour, IUIPanelManagerObserver
    {
        public static MissionChooseManager Instance;

        [Header("UI Visual")]
        [SerializeField, Tooltip("Canvas/Choose mission")] private GameObject MainPanel;
        [SerializeField, Tooltip("Canvas/Choose mission/Scroll View/Content")] private Transform Content;
        [Space]
        [SerializeField] private PlanetUIVisual PlanetVisual;

        [Header("Planets")]
        [SerializeField, Expandable] private PlanetMapSO PlanetMap;
        [SerializeField] private Transform CentralPoint;    

        private Dictionary<Orbit, List<PlanetUIVisual>> allPlanetsOnUI = new Dictionary<Orbit, List<PlanetUIVisual>>();
        private Dictionary<Orbit, float> planetDirection = new Dictionary<Orbit, float>();
        private Dictionary<Orbit, int> planetProgress = new Dictionary<Orbit, int>();
        private bool isOpened = false;
        private Vector2 defaultContentPosition;

        private PlayerController player;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();

            // Input
            GameInput.InputActions.UI.CloseWindow.performed += CloseMenu;

            // Initialize planets
            for (int i = 0; i < PlanetMap.Orbits.Count; i++)
            {
                Orbit orbit = PlanetMap.Orbits[i];

                allPlanetsOnUI.Add(orbit, new List<PlanetUIVisual>());

                planetDirection.Add(orbit, 0);
                planetProgress.Add(orbit, 0);

                foreach (var planet in orbit.PlanetsOnOrbit)
                {
                    PlanetUIVisual visual = Instantiate(PlanetVisual.gameObject, CentralPoint).GetComponent<PlanetUIVisual>();
                    visual.Initialize(planet);

                    allPlanetsOnUI[orbit].Add(visual);
                }
            }

            defaultContentPosition = Content.position;
            UIPanelManager.Instance.Attach(this);
        }

        private void Update()
        {
            UpdateOrbit();
        }

        private void UpdateOrbit()
        {
            foreach (var orbit in PlanetMap.Orbits)
            {
                planetDirection[orbit] += Time.deltaTime * orbit.Speed;

                foreach (var planet in allPlanetsOnUI[orbit])
                {
                    planet.Rotate(CentralPoint, ((float)planetProgress[orbit] / (float)orbit.PlanetsOnOrbit.Count) * 360f + planetDirection[orbit], orbit.DistanceFromPoint);
                    planetProgress[orbit]++;
                }
            }
        }

        #region UI Actions
        public void OpenMenu()
        {
            UIPanelManager.Instance.OpenPanel(MainPanel, false);
            isOpened = true;

            player.canMove = false;
        }
        public void CloseMenu()
        {
            UIPanelManager.Instance.ClosePanel(MainPanel);
            isOpened = false;

            player.canMove = true;
        }
        public void CloseMenu(InputAction.CallbackContext context)
        {
            UIPanelManager.Instance.ClosePanel(MainPanel);
            isOpened = false;

            player.canMove = true;
        }
        #endregion

        private void OnDisable()
        {
            GameInput.InputActions.UI.CloseWindow.performed -= CloseMenu;
        }

        public void PanelStateIsChanged(GameObject panel)
        {
            if (panel == MainPanel)
            {
                if (!MainPanel.activeSelf)
                {
                    return;
                }

                Content.position = defaultContentPosition;
            }
        }
    }
}
