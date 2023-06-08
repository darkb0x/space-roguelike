using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace Game.MainMenu.MissionChoose.Visual
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

        public bool isOpened { get; private set; }
        private Vector2 defaultContentPosition;

        private UIPanelManager UIPanelManager;

        private void Start()
        {
            UIPanelManager = Singleton.Get<UIPanelManager>();

            GameInput.InputActions.UI.CloseWindow.performed += CloseMenu;

            defaultContentPosition = Content.position;
            UIPanelManager.Attach(this);
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

        public void OpenMenu()
        {
            Content.position = defaultContentPosition;

            UIPanelManager.OpenPanel(MainPanel);
            isOpened = true;
        }
        public void CloseMenu()
        {
            if (!isOpened)
                return;

            UIPanelManager.ClosePanel(MainPanel);
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
            if(panel != MainPanel)
            {
                if(isOpened)
                {
                    isOpened = false;
                }
            }
        }
    }
}
