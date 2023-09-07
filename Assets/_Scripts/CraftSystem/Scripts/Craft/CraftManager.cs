using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Craft
{
    using global::CraftSystem.ScriptableObjects;
    using Visual;
    using Save;
    using Inventory;
    using Player;
    using Notifications;

    public class CraftManager : MonoBehaviour, IService, IEntryComponent<PlayerInventory, PlayerController>
    {
        [SerializeField] private bool ShowVisual = true;
        [SerializeField, NaughtyAttributes.ShowIf("ShowVisual")] private CraftVisual Visual;

        private PlayerInventory PlayerInventory;
        private PlayerController Player;
        private SessionSaveData _currentSessionData => SaveManager.SessionSaveData;
        private List<CraftSO> _allUnlockedCrafts;

        private CraftTable _currentCraftTable;

        public void Initialize(PlayerInventory playerInventory, PlayerController player)
        {
            PlayerInventory = playerInventory;
            Player = player;
            _allUnlockedCrafts = _currentSessionData.GetCraftList();

            if(ShowVisual && Visual != null)
            {
                Visual.Initialize(this, _allUnlockedCrafts);
            }
        }

        public void Craft(CraftSO craft)
        {
            if(PlayerInventory.TakeItem(craft.ItemsInCraft))
            {
                GameObject craftedObj = Instantiate(craft.CraftPrefab, Player.transform.position, Quaternion.identity);
                if(craftedObj.TryGetComponent(out ICraftableBuild build))
                {
                    build.Initialize(Player);
                }

                Visual.Close();
                _currentCraftTable = null;

                NotificationManager.NewNotification(
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

            if(ShowVisual && Visual != null)
            {
                Visual.Open();
            }
        }
    }
}