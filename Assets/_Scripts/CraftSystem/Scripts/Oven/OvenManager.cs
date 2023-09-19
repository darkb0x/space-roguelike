using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.CraftSystem.Oven
{
    using Inventory;
    using UI;

    public class OvenManager : MonoBehaviour, IService, IEntryComponent<UIWindowService, Inventory>
    {
        public const WindowID OVEN_WINDOW_ID = WindowID.Oven;

        [SerializeField, Expandable] private OvenConfig craftList;

        Oven currentOven;

        private UIWindowService _uiWindowService;
        private Inventory _inventory;

        public void Initialize(UIWindowService windowService, Inventory inventory)
        {
            _uiWindowService = windowService;
            _inventory = inventory;

            _uiWindowService.RegisterWindow<OvenVisual>(OVEN_WINDOW_ID).Initialize(this, craftList.Items);
        }

        public void RemeltingItem(OvenConfig.craft craft)
        {
            List<ItemData> itemsData = ConvertOvenCraftToItemData(craft);

            if (_inventory.TakeItem(itemsData))
            {
                currentOven.StartRemelting(craft);
                _uiWindowService.Close(OVEN_WINDOW_ID);
            }
        }

        private List<ItemData> ConvertOvenCraftToItemData(OvenConfig.craft craft)
        {
            List<ItemData> itemsData = new List<ItemData>();
            foreach (var item in craft.firstItems)
            {
                itemsData.Add(item);
            }
            return itemsData;
        }

        public void Open(Oven oven)
        {
            currentOven = oven;
            _uiWindowService.Open(OVEN_WINDOW_ID);
        }
    }
}
