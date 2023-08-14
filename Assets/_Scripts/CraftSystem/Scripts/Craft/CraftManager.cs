using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Craft
{
    using global::CraftSystem.ScriptableObjects;
    using Visual;
    using SaveData;
    using Player.Inventory;
    using Player;

    public class CraftManager : MonoBehaviour, ISingleton
    {
        [SerializeField] private bool ShowVisual = true;
        [SerializeField, NaughtyAttributes.ShowIf("ShowVisual")] private CraftVisual Visual;

        private PlayerInventory PlayerInventory;
        private PlayerController Player;
        private SessionData _currentSessionData => SaveDataManager.Instance.CurrentSessionData;
        private List<CraftSO> _allUnlockedCrafts;

        private CraftTable _currentCraftTable;

        private void Awake()
        {
            Singleton.Add(this);
        }

        private void Start()
        {
            PlayerInventory = Singleton.Get<PlayerInventory>();
            Player = FindObjectOfType<PlayerController>();
            _allUnlockedCrafts = _currentSessionData.GetCraftList();

            if(ShowVisual && Visual != null)
            {
                Visual.Initialize(this, _allUnlockedCrafts);
            }
        }

        public void Craft(CraftSO craft)
        {
            if(PlayerInventory.CanTakeItems(craft.ItemsInCraft))
            {
                GameObject craftedObj = Instantiate(craft.CraftPrefab, Player.transform.position, Quaternion.identity);
                if(craftedObj.TryGetComponent(out ICraftableBuild build))
                {
                    build.Initialize(Player);
                }

                PlayerInventory.TakeItem(craft.ItemsInCraft, false);

                Visual.Close();
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