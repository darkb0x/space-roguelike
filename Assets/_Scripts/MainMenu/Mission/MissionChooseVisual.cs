using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using TMPro;

namespace Game.MainMenu.Mission.Visual
{
    public class MissionChooseVisual : MonoBehaviour, IUIPanelManagerObserver
    {
        [Header("Main")]
        [SerializeField] private GameObject MainPanel;
        [SerializeField] private Transform Content;

        [Header("Mission Tab")]
        [SerializeField] private GameObject SelectedMissionPanel;
        [SerializeField] private Image SelectedMissionIcon;
        [SerializeField] private TextMeshProUGUI SelectedMissionNameText;

        [Header("Start Mission Timer")]
        [SerializeField] private TextMeshProUGUI StartMissionTimerText;

        private bool isOpened = false;
        private Vector2 defaultContentPosition;

        private void Start()
        {
            GameInput.InputActions.UI.CloseWindow.performed += CloseMenu;

            defaultContentPosition = Content.position;
            UIPanelManager.Instance.Attach(this);
        }

        public void ShowMissionTab(Sprite missionIcon, string missionName)
        {
            SelectedMissionIcon.sprite = missionIcon;
            SelectedMissionNameText.text = missionName;

            SelectedMissionPanel.SetActive(true);
        }
        public void HideMissionTab()
        {
            SelectedMissionPanel.SetActive(false);
        }

        public void ShowStartMissionTimer(float time)
        {
            StartMissionTimerText.text = "Mission starts in " + time.ToString("F1") + " seconds!";
            StartMissionTimerText.gameObject.SetActive(true);
        }
        public void HideStartMissionTimer()
        {
            StartMissionTimerText.gameObject.SetActive(false);
        }

        public void OpenMenu()
        {
            UIPanelManager.Instance.OpenPanel(MainPanel, false);
            isOpened = true;
        }
        public void CloseMenu()
        {
            UIPanelManager.Instance.ClosePanel(MainPanel);
            isOpened = false;
        }
        public void CloseMenu(InputAction.CallbackContext context)
        {
            CloseMenu();
        }
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
