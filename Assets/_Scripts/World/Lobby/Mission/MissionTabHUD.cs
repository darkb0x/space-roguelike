using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Lobby.Missions
{
    using Game.UI.HUD;

    public class MissionTabHUD : HUDElement
    {
        [SerializeField] private Image SelectedMissionIcon;
        [Space]
        [SerializeField] private TextMeshProUGUI SelectedMissionNameText;
        [Space]
        [SerializeField] private Transform SelectedMissionUniqueItemsContent;
        [SerializeField] private GameObject UniqueItemPrefab;
        [SerializeField] private Sprite UniqueItemAmountMax;
        [SerializeField] private Sprite UniqueItemAmountMiddle;
        [SerializeField] private Sprite UniqueItemAmountLow;

        public override HUDElementID ID => HUDElementID.MissionTab;

        private MissionChooseManager _missionManager;

        public override void Initialize()
        {
            _missionManager = ServiceLocator.GetService<MissionChooseManager>();

            base.Initialize();

            Hide();
        }

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            _missionManager.OnMissionSelected += UpdateView;
            _missionManager.OnMissionTabShowed += Show;
            _missionManager.OnMissionTabHided += Hide;
        }
        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            _missionManager.OnMissionSelected -= UpdateView;
            _missionManager.OnMissionTabShowed -= Show;
            _missionManager.OnMissionTabHided -= Hide;
        }

        private void UpdateView(PlanetSO planetSO)
        {
            SelectedMissionIcon.sprite = planetSO.MissionIcon;
            SelectedMissionNameText.text = planetSO.MissionName;

            if (SelectedMissionUniqueItemsContent.childCount > 0)
            {
                int childCount = SelectedMissionUniqueItemsContent.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(SelectedMissionUniqueItemsContent.GetChild(i).gameObject);
                }
            }
            foreach (var item in planetSO.UniqueItems)
            {
                GameObject itemObj = Instantiate(UniqueItemPrefab, SelectedMissionUniqueItemsContent);
                Image itemIcon = itemObj.transform.GetChild(1).GetComponent<Image>();
                itemIcon.sprite = item.Item.LowSizeIcon;
                Image itemBgIcon = itemObj.transform.GetChild(0).GetComponent<Image>();

                if (item.PercentInWorld >= 8)
                {
                    itemBgIcon.sprite = UniqueItemAmountMax;
                }
                else if (item.PercentInWorld < 8 && item.PercentInWorld > 3)
                {
                    itemBgIcon.sprite = UniqueItemAmountMiddle;
                }
                else if (item.PercentInWorld <= 3)
                {
                    itemBgIcon.sprite = UniqueItemAmountLow;
                }
            }
        }
    }
}