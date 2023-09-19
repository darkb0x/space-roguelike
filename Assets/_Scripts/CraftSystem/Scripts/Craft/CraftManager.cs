using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Craft
{
    using global::CraftSystem.ScriptableObjects;
    using Visual;
    using Save;
    using UI;
    using Player;
    using Inventory;
    using Notifications;

    public class CraftManager : MonoBehaviour, IService, IEntryComponent<PlayerInventory, PlayerController, UIWindowService>
    {
        public const WindowID CRAFT_WINDOW_ID = WindowID.Craft;

        [SerializeField] private bool ShowVisual = true;

        private SessionSaveData _currentSessionData => SaveManager.SessionSaveData;
        private UIWindowService _uiWindowService;

        private Inventory _inventory;
        private PlayerController _player;

        private List<CraftSO> _allUnlockedCrafts;
        private CraftTable _currentCraftTable;

        public void Initialize(PlayerInventory playerInventory, PlayerController player, UIWindowService windowService)
        {
            _inventory = playerInventory;
            _player = player;
            _uiWindowService = windowService;

            _allUnlockedCrafts = _currentSessionData.GetCraftList();

            if(ShowVisual)
            {
                _uiWindowService.RegisterWindow<CraftVisual>(CRAFT_WINDOW_ID)
                    .Initialize(this, _allUnlockedCrafts);
            }
        }

        public void Craft(CraftSO craft)
        {
            if(_inventory.TakeItem(craft.ItemsInCraft))
            {
                GameObject craftedObj = Instantiate(craft.CraftPrefab, _player.transform.position, Quaternion.identity);
                if(craftedObj.TryGetComponent(out ICraftableBuild build))
                {
                    build.Initialize(_player);
                }

                _uiWindowService.Close(CRAFT_WINDOW_ID);
                _currentCraftTable = null;

                NotificationService.NewNotification(
                    craft.CraftIcon,
                    "Crafted",
                    true,
                    Color.white
                    );
            }
            else
            {
                // to do
            }
        }

        public void Open(CraftTable craftTable)
        {
            _currentCraftTable = craftTable;

            if(ShowVisual)
            {
                _uiWindowService.Open(CRAFT_WINDOW_ID);
            }
        }
    }
}