using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Save
{
    using Inventory;
    using MainMenu.MissionChoose.Planet;
    using global::CraftSystem.ScriptableObjects;
    using System.Linq;
    using NaughtyAttributes;

    [Serializable]
    public class SessionSaveData : SaveData
    {
        // Crafts
        public List<string> UnlockedCraftPaths;

        // Inventory
        public InventorySaveData MainInventory;
        public InventorySaveData LobbyInventory;
        public int Money;

        // Resource Automat Uses Amount
        public int ResourceAutomatCurrentInteract;

        // Planet
        public string PlanetPath;

        // Difficult
        public float CurrentDifficultFactor;

        public SessionSaveData(string fileName, string filePath) : base(fileName, filePath, false)
        {
            UnlockedCraftPaths = new List<string>();

            MainInventory = new InventorySaveData();
            LobbyInventory = new InventorySaveData();

            Reset();
        }

        public override void Reset()
        {
            UnlockedCraftPaths.Clear();

            MainInventory.Clear();
            LobbyInventory.Clear();

            Money = 0;
            ResourceAutomatCurrentInteract = 0;

            PlanetPath = "";
            CurrentDifficultFactor = 1;
        }

        #region Craft
        public void InjectCrafts(List<CraftSO> crafts)
        {
            UnlockedCraftPaths.Clear();
            UnlockedCraftPaths.AddRange(crafts.ConvertAll(x => x.AssetPath));
        }
        public CraftSO GetCraft(string path)
        {
            if (UnlockedCraftPaths.Any(x => x == path))
                return Resources.Load<CraftSO>(path);

            return null; 
        }
        public List<CraftSO> GetCraftList()
            => UnlockedCraftPaths.ConvertAll(x => GetCraft(x));
        public bool ContainsCraft(CraftSO craft)
            => UnlockedCraftPaths.Contains(craft.AssetPath);
        #endregion

        #region Planet
        public void SetPlanet(PlanetSO planet)
        {
            PlanetPath = planet.AssetPath;
        }
        public PlanetSO GetPlanet()
        {
            try
            {
                return Resources.Load<PlanetSO>(PlanetPath);
            }
            catch (Exception)
            {
                Debug.LogError("Cant find planet at path: " + PlanetPath);
                return null;
            }
        }
        #endregion

        #region Inventory
        [Serializable]
        public class InventorySaveData
        {
            public List<ItemSaveData> Items;

            public InventorySaveData()
            {
                Items = new List<ItemSaveData>();
            }

            public void SetItem(ItemData data)
            {
                var item = Items.FirstOrDefault(x => x.Path == data.Item.AssetPath);
                if(item != null)
                {
                    item.Amount = data.Amount;
                    return;
                }

                Items.Add(new ItemSaveData(data));
            }
            public List<ItemData> GetItemList()
            {
                var result = new List<ItemData>();
                foreach (var item in Items)
                {
                    var inventoryItem = Resources.Load<InventoryItem>(item.Path);
                    result.Add(new ItemData(inventoryItem, item.Amount));
                }
                return result;
            }
            public void Clear()
            {
                Items.Clear();
            }

            [Serializable]
            public class ItemSaveData
            {
                [SerializeField] private string Name;
                [ShowNonSerializedField] public string Path;
                public int Amount;

                public ItemSaveData(ItemData data)
                {
                    Path = data.Item.AssetPath;
                    Name = data.Item.ItemName;
                    Amount = data.Amount;
                }
            }
        }
        #endregion
    }
}
