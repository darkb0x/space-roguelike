using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SaveData
{
    using Inventory;
    using Utilities;
    using MainMenu.MissionChoose.Planet;
    using global::CraftSystem.ScriptableObjects;
    using System.Linq;

    [Serializable]
    public class SessionData : Data
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

        public SessionData(string savePath, string fileName)
        {
            UnlockedCraftPaths = new List<string>();

            MainInventory = new InventorySaveData();
            LobbyInventory = new InventorySaveData();

            dataSavePath = savePath;
            dataFileName = fileName;

            Reset();
        }

        #region Craft
        public void InjectCrafts(List<CraftSO> crafts)
        {
            UnlockedCraftPaths.Clear();
            foreach (var craft in crafts)
            {
                UnlockedCraftPaths.Add(craft.AssetPath);
            }
        }
        public CraftSO GetCraft(string path)
        {
            foreach (var craft in UnlockedCraftPaths)
            {
                if (craft == path)
                {
                    CraftSO craftSO = Resources.Load<CraftSO>(craft);
                    return craftSO;
                }
            }
            return null; 
        }
        public List<CraftSO> GetCraftList()
        {
            List<CraftSO> result = new List<CraftSO>();

            foreach (var path in UnlockedCraftPaths)
            {
                result.Add(GetCraft(path));
            }

            return result;
        }
        public bool ContainsCraft(string path)
        {
            return GetCraft(path) != null;
        }
        public bool ContainsCraft(CraftSO craft)
        {
            return ContainsCraft(craft.AssetPath);
        }
        #endregion

        #region Planet
        public void SetPlanet(string path)
        {
            PlanetPath = path;
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

        public override void Save(string filePath, string fileName)
        {
            dataSavePath = filePath;
            dataFileName = fileName;

            Save();

        }
        public override void Save()
        {
            SaveUtility.SaveDataToJson(dataSavePath, dataFileName, this, true);
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

            base.Reset();
        }

        [Serializable]
        public class InventorySaveData
        {
            public List<Item> Items;

            public InventorySaveData()
            {
                Items = new List<Item>();
            }

            private void AddItem(ItemData data)
            {
                Items.Add(new Item() { Path = data.Item.AssetPath, Amount = data.Amount });
            }
            public void SetItem(ItemData data)
            {
                var item = Items.FirstOrDefault(x => x.Path == data.Item.AssetPath);
                if(item != null)
                {
                    item.Amount = data.Amount;
                    return;
                }

                AddItem(data);
            }
            public ItemData GetItem(string path)
            {
                foreach (var item in Items)
                {
                    if (item.Path == path)
                    {
                        InventoryItem inventoryItem = Resources.Load<InventoryItem>(item.Path);
                        ItemData data = new ItemData(inventoryItem, item.Amount);
                        return data;
                    }
                }
                Debug.LogWarning($"Can't find '{path}'!");
                return null;
            }
            public List<ItemData> GetItemList()
            {
                List<ItemData> itemDatas = new List<ItemData>();
                foreach (var item in Items)
                {
                    itemDatas.Add(GetItem(item.Path));
                }
                return itemDatas;
            }
            public void Clear()
            {
                Items.Clear();
            }

            [Serializable]
            public class Item
            {
                public string Path;
                public int Amount;
            }
        }
    }
}
