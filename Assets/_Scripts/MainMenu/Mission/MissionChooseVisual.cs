using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.MainMenu.MissionChoose.Visual
{
    using Input;

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
        [SerializeField] private GameObject UniqueItemPrefab;
        [SerializeField] private Sprite UniqueItemAmountMax;
        [SerializeField] private Sprite UniqueItemAmountMiddle;
        [SerializeField] private Sprite UniqueItemAmountLow;

        public bool isOpened { get; private set; }
        private Vector2 defaultContentPosition;

        private UIPanelManager UIPanelManager;
        private UIInputHandler _input => InputManager.UIInputHandler;

        private void Start()
        {
            UIPanelManager = ServiceLocator.GetService<UIPanelManager>();

            _input.CloseEvent += CloseMenu;

            defaultContentPosition = Content.position;
            UIPanelManager.Attach(this);
        }
        private void OnDisable()
        {
            _input.CloseEvent -= CloseMenu;
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
                GameObject itemObj = Instantiate(UniqueItemPrefab, SelectedMissionUniqueItemsContent);
                Image itemIcon = itemObj.transform.GetChild(1).GetComponent<Image>();
                itemIcon.sprite = item.Item.LowSizeIcon;
                Image itemBgIcon = itemObj.transform.GetChild(0).GetComponent<Image>();

                if (item.PercentInWorld >= 8)
                {
                    itemBgIcon.sprite = UniqueItemAmountMax;
                }
                else if(item.PercentInWorld < 8 && item.PercentInWorld > 3)
                {
                    itemBgIcon.sprite = UniqueItemAmountMiddle;
                }
                else if(item.PercentInWorld <= 3)
                {
                    itemBgIcon.sprite = UniqueItemAmountLow;
                }
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
