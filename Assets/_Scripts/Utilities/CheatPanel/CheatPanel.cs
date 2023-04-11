using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Game.Utilities.CheatPanel
{
    using Visual;
    using Player;
    using Player.Inventory;

    public class CheatPanel : MonoBehaviour
    {
        [SerializeField] private CheatPanelVisual Visual;
        [Space]
        [SerializeField] private PlayerController PlayerController;
        [Space]
        [SerializeField] private PlayerCheatPanel PlayerCheatPanel;
        [SerializeField] private CraftsCheatPanel CraftsCheatPanel;
        [SerializeField] private EnemyCheatPanel EnemyCheatPanel;
        [SerializeField] private WorldCheatPanel WorldCheatPanel;

        private void Start()
        {
            PlayerCheatPanel.Initialize(PlayerController);
            CraftsCheatPanel.Initialize();
            EnemyCheatPanel.Initialize();
            WorldCheatPanel.Initialize();
        }

        public static class Utilities
        {
            public static void CreateButton_Icon(string name, Vector2 size, Sprite icon, Transform parent, UnityAction onClickEvent)
            {
                GameObject buttonObject = new GameObject();
                RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
                Image buttonVisual = buttonObject.AddComponent<Image>();
                Button buttonComponent = buttonObject.AddComponent<Button>();

                buttonObject.name = name;
                buttonRect.sizeDelta = size;
                buttonVisual.sprite = icon;
                buttonVisual.preserveAspect = true;
                buttonComponent.onClick.AddListener(onClickEvent);

                buttonObject.transform.SetParent(parent);
                buttonObject.transform.localScale = Vector3.one;
            }
        }
    }

    [System.Serializable]
    public class PlayerCheatPanel
    {
        [SerializeField] private Toggle DoHealthCycleToggle;
        [SerializeField] private Toggle DoOxygenCycleToggle;
        [Space]
        [SerializeField] private Button GiveMoneyButton;
        [SerializeField] private TMP_InputField GiveMoneyInputField;
        [Space]
        [SerializeField] private Button GiveItemButton;
        [SerializeField] private TMP_InputField GiveItemInputField;
        [SerializeField] private Button ChooseItemButton;
        [SerializeField] private Image ChoosedItemVisual;

        [Header("Item Selection Panel")]
        [SerializeField] private List<InventoryItem> Items = new List<InventoryItem>();
        [SerializeField] private Transform ItemVisualsParent;
        [SerializeField] private GameObject ItemListPanel;

        private InventoryItem choosedItem;
        private PlayerController player;
        private bool itemSelectionPanelIsOpened;

        public void Initialize(PlayerController player)
        {
            this.player = player;
            itemSelectionPanelIsOpened = false;

            DoHealthCycleToggle.onValueChanged.AddListener(value => { EnableHealthCycle(value); });
            DoHealthCycleToggle.isOn = player.DoHealthCycle;
            DoOxygenCycleToggle.onValueChanged.AddListener(value => { EnableOxygenCycle(value); });
            DoOxygenCycleToggle.isOn = player.DoOxygenCycle;

            GiveMoneyButton.onClick.AddListener(() => GiveMoney(GiveMoneyInputField.text));

            GiveItemButton.onClick.AddListener(() => GiveItem(GiveItemInputField.text));
            ChooseItemButton.onClick.AddListener(() => OpenCloseItemSelectionPanel());
            ChoosedItemVisual.color = new Color(1, 1, 1, 0);

            InitializeItemSelectioPanel();
        }
        private void InitializeItemSelectioPanel()
        {
            ItemListPanel.SetActive(false);

            Vector2 buttonSize = new Vector2(100f, 100f);

            for (int i = 0; i < Items.Count; i++)
            {
                InventoryItem item = Items[i];

                CheatPanel.Utilities.CreateButton_Icon($"Item: {item.ItemName} ({i})", buttonSize, item.Icon, ItemVisualsParent, () =>
                {                   
                    choosedItem = item;
                    ChoosedItemVisual.sprite = choosedItem.Icon;
                    ChoosedItemVisual.color = new Color(1, 1, 1, 1);

                    itemSelectionPanelIsOpened = false;
                    ItemListPanel.SetActive(itemSelectionPanelIsOpened);
                });
            }
        }
        private void OpenCloseItemSelectionPanel()
        {
            itemSelectionPanelIsOpened = !itemSelectionPanelIsOpened;

            ItemListPanel.SetActive(itemSelectionPanelIsOpened);
        }

        private void EnableHealthCycle(bool enabled)
        {
            player.DoHealthCycle = enabled;
            LogUtility.WriteLog($"Cheats. Player.DoHealthCycle = {enabled}");
        }
        private void EnableOxygenCycle(bool enabled)
        {
            player.DoOxygenCycle = enabled;
            LogUtility.WriteLog($"Cheats. Player.DoOxygenCycle = {enabled}");
        }

        private void GiveMoney(string amountTextField)
        {
            int amount = 0;
            foreach (var symbol in amountTextField)
            {
                if(char.IsLetter(symbol))
                {
                    Debug.Log($"Cheats. Can't add: {amountTextField} to money. Field have a letters");
                    return;
                }
            }
            amount = int.Parse(amountTextField);

            PlayerInventory.Instance.money += amount;
            LogUtility.WriteLog($"Cheats. PlayerInventory.money + {amount}");
        }
        private void GiveItem(string amountTextField)
        {
            if(choosedItem == null)
            {
                Debug.Log($"Cheats. Any item hadn't be choosed. I can't add it to inventory!");
                return;
            }

            int amount = 0;
            foreach (var symbol in amountTextField)
            {
                if (char.IsLetter(symbol))
                {
                    Debug.Log($"Cheats. Can't add: '{amountTextField}' to items. Field have a letters");
                    return;
                }
            }
            amount = int.Parse(amountTextField);

            PlayerInventory.Instance.AddItem(choosedItem, amount);
            LogUtility.WriteLog($"Cheats. PlayerInventory.AddItem({choosedItem.ItemName}, {amount})");
        }
    }

    [System.Serializable]
    public class CraftsCheatPanel
    {
        public void Initialize()
        {
            
        }
    }

    [System.Serializable]
    public class EnemyCheatPanel
    {
        public void Initialize()
        {

        }
    }

    [System.Serializable]
    public class WorldCheatPanel
    {
        public void Initialize()
        {

        }
    }
}
