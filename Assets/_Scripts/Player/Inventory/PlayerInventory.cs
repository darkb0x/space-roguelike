using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

namespace Game.Player.Inventory
{
    using SaveData;

    public interface IInventoryObserver
    {
        public void UpdateData(PlayerInventory inventory);
    }

    [System.Serializable]
    public class ItemData
    {
        [Expandable] public InventoryItem Item;
        public int Amount;
    }

    public class PlayerInventory : MonoBehaviour
    {
        // singletone
        public static PlayerInventory Instance;
        private void Awake() => Instance = this;

        [System.Serializable]
        private class ItemVisual
        {
            [Expandable] public InventoryItem item;
            public int amount;

            [HideInInspector] public Image UI_icon;
            [HideInInspector] public TextMeshProUGUI UI_amount;

            public void Add(int value)
            {
                this.amount += value;
            }
            public void Take(int value)
            {
                this.amount -= value;
            }
            public void InitializeVisual(Image img, TextMeshProUGUI text)
            {
                UI_icon = img;
                UI_amount = text;
            }
        }

        [Header("Inventory")]
        [SerializeField] private List<ItemVisual> itemsVisuals = new List<ItemVisual>();
        public List<IInventoryObserver> observers = new List<IInventoryObserver>();
        public int money
        {
            get
            {
                return m_money;
            }
            set
            {
                m_money = value;
                foreach (var text in money_texts)
                {
                    text.text = m_money + "$";
                }
            }
        }
        [SerializeField] private int m_money;
        [SerializeField] private TextMeshProUGUI[] money_texts;

        [Header("UI")]
        [SerializeField, Tooltip("Canvas/Player/Items")] private Transform uiElements_tranform;
        [SerializeField] private GameObject itemInUI_prefab;
        [Space]
        [SerializeField] private InventoryScreen[] InventoryScreens;
        [Space]
        [SerializeField] private Animator InventoryAnim;
        [SerializeField] private TextMeshProUGUI AllItemsAmountText;
        [SerializeField, AnimatorParam("InventoryAnim")] private string InventoryAnim_OpenBool;

        private bool isOpened = false;
        private GameData.SessionData currentSessionData => GameData.Instance.CurrentSessionData;

        private void Start()
        {
            GameInput.InputActions.Player.Inventory.performed += InventoryEnabled;

            Load();

            for (int i = 0; i < itemsVisuals.Count; i++)
            {
                Transform obj = Instantiate(itemInUI_prefab, uiElements_tranform).transform;
                itemsVisuals[i].InitializeVisual(obj.GetChild(0).GetComponent<Image>(), obj.GetChild(1).GetComponent<TextMeshProUGUI>());
            }

            UpdateVisual();
        }
        private void Update()
        {
            InventoryAnim.SetBool(InventoryAnim_OpenBool, isOpened);
        }

        private void Load()
        {
            money = currentSessionData.Money;

            if(currentSessionData.Items.Keys.Count == 0)
            {
                foreach (var visual in itemsVisuals)
                {
                    ItemData data = ConvertVisualToItem(visual);
                    data.Amount = 0;

                    currentSessionData.AddItem(data);
                }

                GameData.Instance.Save();
            }

            foreach (var item in currentSessionData.Items.Keys)
            {
                ItemData data = currentSessionData.GetItem(item);
                ConvertItemToVisual(data).amount = data.Amount;
            }
        }

        public ItemData GetItem(InventoryItem item)
        {
            foreach (var visual in itemsVisuals)
            {
                if (visual.item == item)
                    return ConvertVisualToItem(visual);
            }
            return null;
        }

        /// <summary>
        /// Give an item to inventory
        /// </summary>
        /// <param name="item">item to give</param>
        /// <param name="amount">amount</param>
        public void GiveItem(InventoryItem item, int amount)
        {
            ConvertItemToVisual(GetItem(item)).Add(amount);

            UpdateVisual();
        }

        /// <summary>
        /// Take item from inventory
        /// </summary>
        /// <param name="item">Item to take</param>
        /// <param name="amount">Amount</param>
        public void TakeItem(InventoryItem item, int amount)
        {
            ItemVisual it = ConvertItemToVisual(GetItem(item));
            if (it.amount >= amount)
            {
                it.Take(amount);
                UpdateVisual();
            }
        }
        /// <summary>
        /// Take items from inventory
        /// </summary>
        /// <param name="items">List of items for take</param>
        public void TakeItem(List<ItemData> items)
        {
            foreach (var item in items)
            {
                TakeItem(item.Item, item.Amount);
            }
        }

        public bool CanTakeItems(List<ItemData> items)
        {
            foreach (var item in items)
            {
                if (GetItem(item.Item).Amount < item.Amount)
                    return false;
            }
            return true;
        }

        #region Utilities
        private ItemData ConvertVisualToItem(ItemVisual visual)
        {
            ItemData item = new ItemData
            {
                Item = visual.item,
                Amount = visual.amount
            };

            return item;
        }
        private ItemVisual ConvertItemToVisual(ItemData item)
        {
            foreach (var visual in itemsVisuals)
            {
                if (visual.item == item.Item)
                    return visual;
            }
            return null;
        }

        private void OnDisable()
        {
            GameInput.InputActions.Player.Inventory.performed -= InventoryEnabled;
        }

        #region UI Visual
        private void UpdateVisual()
        {
            List<ItemData> data = new List<ItemData>();
            int allItemsAmount = 0;

            // main
            foreach (var item in itemsVisuals)
            {
                item.UI_icon.sprite = item.item.Icon;
                item.UI_amount.text = item.amount.ToString();

                allItemsAmount += item.amount;

                data.Add(ConvertVisualToItem(item));
            }
            AllItemsAmountText.text = allItemsAmount.ToString();

            // screens data
            foreach (var screen in InventoryScreens)
            {
                screen.UpdateData(data);
            }

            // observers data
            foreach (var observer in observers)
            {
                observer.UpdateData(this);
            }
        }

        public void InventoryEnabled()
        {
            isOpened = !isOpened;
        }
        public void InventoryEnabled(UnityEngine.InputSystem.InputAction.CallbackContext callback)
        {
            InventoryEnabled();
        }
        #endregion
        #endregion
    }
}
