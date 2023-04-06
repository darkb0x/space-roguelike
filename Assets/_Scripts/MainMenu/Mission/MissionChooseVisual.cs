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
        [Space]
        [SerializeField] private Image SelectedMissionIcon;
        [Space]
        [SerializeField] private TextMeshProUGUI SelectedMissionNameText;
        [Space]
        [SerializeField] private Transform SelectedMissionUniqueItemsContent;
        [SerializeField] private Vector2 UniqueItemsSize = new Vector2(100, 100);

        [Header("Start Mission Timer")]
        [SerializeField] private TextMeshProUGUI StartMissionTimerText;

        public bool isOpened { get; private set; }
        private Vector2 defaultContentPosition;

        private void Start()
        {
            GameInput.InputActions.UI.CloseWindow.performed += CloseMenu;

            defaultContentPosition = Content.position;
            UIPanelManager.Instance.Attach(this);
        }

        public void ShowMissionTab(Sprite missionIcon, string missionName, List<Planet.PlanetSO.ItemGenerationData> items)
        {
            SelectedMissionIcon.sprite = missionIcon;
            SelectedMissionNameText.text = missionName;

            if(SelectedMissionUniqueItemsContent.childCount > 0)
            {
                int childCount = SelectedMissionUniqueItemsContent.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(SelectedMissionUniqueItemsContent.GetChild(i).gameObject);
                }
            }
            foreach (var item in items)
            {
                GameObject itemObj = new GameObject();

                itemObj.transform.SetParent(SelectedMissionUniqueItemsContent);
                itemObj.transform.localScale = Vector3.one;

                Image itemImage = itemObj.AddComponent<Image>();
                itemImage.sprite = item.Item.Icon;

                itemImage.rectTransform.sizeDelta = UniqueItemsSize;
            }

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
            UIPanelManager.Instance.OpenPanel(MainPanel);
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
