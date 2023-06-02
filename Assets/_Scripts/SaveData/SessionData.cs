using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SaveData
{
    using Player.Inventory;
    using Utilities;
    using MainMenu.Mission.Planet;
    using CraftSystem.Editor.ScriptableObjects;

    [Serializable]
    public class SessionData : Data
    {

        // Crafts
        public List<string> UnlockedCraftPaths;

        // Inventory
        public Inventory MainInventory;
        public Inventory LobbyInventory;
        public int Money;

        // Resource Automat Uses Amount
        public int ResourceAutomatCurrentInteract;

        // Planet
        public string PlanetPath;

        public SessionData(string savePath, string fileName)
        {
            dataSavePath = savePath;
            dataFileName = fileName;

            Reset();
        }

        #region Craft
        public CSCraftSO GetCraft(string path)
        {
            foreach (var craft in UnlockedCraftPaths)
            {
                if (craft == path)
                {
                    CSCraftSO craftSO = Resources.Load<CSCraftSO>(craft);
                    return craftSO;
                }
            }
            return null;
        }
        public bool HaveCraft(CSCraftSO craft)
        {
            return GetCraft(craft.AssetPath) != null;
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

            SaveDataUtility.SaveDataToJson(this, fileName, filePath, true);

        }
        public override void Save()
        {
            SaveDataUtility.SaveDataToJson(this, dataFileName, dataSavePath, true);
        }
        public override void Reset()
        {
            UnlockedCraftPaths = new List<string>();

            MainInventory = new Inventory();
            LobbyInventory = new Inventory();

            Money = 0;
            ResourceAutomatCurrentInteract = 0;

            PlanetPath = "";

            base.Reset();
        }

        [Serializable]
        public class Inventory
        {
            public List<Item> Items;

            public Inventory()
            {
                Items = new List<Item>();
            }

            public void AddItem(ItemData data)
            {
                foreach (var item in Items)
                {
                    if (item.Path == data.Item.AssetPath)
                        return;
                }
                Items.Add(new Item() { Path = data.Item.AssetPath, Amount = data.Amount });
            }
            public void SetItem(ItemData data)
            {
                foreach (var item in Items)
                {
                    if (item.Path == data.Item.AssetPath)
                    {
                        item.Amount = data.Amount;
                        return;
                    }
                }
                Items.Add(new Item() { Path = data.Item.AssetPath, Amount = data.Amount });
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
                    InventoryItem inventoryItem = Resources.Load<InventoryItem>(item.Path);
                    ItemData data = new ItemData(inventoryItem, item.Amount);
                    itemDatas.Add(data);
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
