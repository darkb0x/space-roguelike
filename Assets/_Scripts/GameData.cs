using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using System.IO;
using AYellowpaper.SerializedCollections;

namespace Game.SaveData
{
    using Player.Inventory;
    using Utilities;
    using MainMenu.Mission.Planet;
    using CraftSystem.Editor.ScriptableObjects;

    public class GameData : MonoBehaviour
    {
        public static GameData Instance;

        public const string SESSION_DATA_FILENAME = "SessionData";
        public const string SETTINGS_DATA_FILENAME = "SettingsData";
        [ReadOnly] public string savePath;

        public SessionData CurrentSessionData;
        public SettingsData CurrentSettingsData;

        private void Awake()
        {
            Instance = this;

            savePath = $"{Application.dataPath}/";
            
            CurrentSessionData = (SessionData)SaveDataUtility.LoadData<SessionData>(savePath, SESSION_DATA_FILENAME, new SessionData(savePath, SESSION_DATA_FILENAME));
            CurrentSettingsData = (SettingsData)SaveDataUtility.LoadData<SettingsData>(savePath, SETTINGS_DATA_FILENAME, new SettingsData(savePath, SETTINGS_DATA_FILENAME));
        }

        public void ResetSessionData()
        {
            CurrentSessionData = new SessionData(savePath, SESSION_DATA_FILENAME);
            CurrentSessionData.Save();
        }
        public void ResetSettingsData()
        {
            CurrentSettingsData = new SettingsData(savePath, SETTINGS_DATA_FILENAME);
            CurrentSettingsData.Save();
        }

        [Button]
        public void SaveAllData()
        {
            CurrentSessionData.Save(savePath, SESSION_DATA_FILENAME);
            CurrentSettingsData.Save(savePath, SETTINGS_DATA_FILENAME);
        }
    }

    public abstract class Data
    {
        public string dataSavePath;
        public string dataFileName;

        public virtual void Save(string filePath, string fileName) { }
        public virtual void Save() { }
    }
    [Serializable]
    public class SessionData : Data
    {
        // Crafts
        public List<string> UnlockedCraftPaths;

        // Inventory
        public Inventory MainInventory;
        public Inventory LobbyInventory;
        public int Money;

        // Planet
        public PlanetSO Planet;

        public SessionData(string savePath, string fileName)
        {
            dataSavePath = savePath;
            dataFileName = fileName;

            UnlockedCraftPaths = new List<string>();

            MainInventory = new Inventory();
            LobbyInventory = new Inventory();

            Money = 0;

            Planet = null;
        }

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

        public override void Save(string filePath, string fileName)
        {
            base.Save(filePath, fileName);

            dataSavePath = filePath;
            dataFileName = fileName;

            SaveDataUtility.SaveData(this, fileName, filePath);
        }
        public override void Save()
        {
            base.Save();

            SaveDataUtility.SaveData(this, dataFileName, dataSavePath);
        }

        [Serializable]
        public class Inventory
        {
            [SerializedDictionary("Item path", "Item amount")]
            public SerializedDictionary<string, int> Items;

            public Inventory()
            {
                Items = new SerializedDictionary<string, int>();
            }

            public void AddItem(ItemData data)
            {
                foreach (var item in Items)
                {
                    if (item.Key == data.Item.AssetPath)
                        return;
                }
                Items.Add(data.Item.AssetPath, data.Amount);
            }
            public void SetItem(ItemData data)
            {
                foreach (var item in Items.Keys)
                {
                    if (item == data.Item.AssetPath)
                    {
                        Items[item] = data.Amount;
                        return;
                    }
                }
                Items.Add(data.Item.AssetPath, data.Amount);
            }
            public ItemData GetItem(string path)
            {
                foreach (var item in Items.Keys)
                {
                    if (item == path)
                    {
                        InventoryItem inventoryItem = Resources.Load<InventoryItem>(item);
                        ItemData data = new ItemData(inventoryItem, Items[item]);
                        return data;
                    }
                }
                Debug.LogWarning($"Can't find '{path}'!");
                return null;
            }
            public List<ItemData> GetItemList()
            {
                List<ItemData> itemDatas = new List<ItemData>();
                foreach (var item in Items.Keys)
                {
                    InventoryItem inventoryItem = Resources.Load<InventoryItem>(item);
                    ItemData data = new ItemData(inventoryItem, Items[item]);
                    itemDatas.Add(data);
                }
                return itemDatas;
            }
            public void Clear()
            {
                Items.Clear();
            }
        }
    }

    [Serializable]
    public class SettingsData : Data
    {
        public int MaxFps = 60;

        public SettingsData(string savePath, string fileName)
        {
            dataSavePath = savePath;
            dataFileName = fileName;

            MaxFps = 60;
        }

        public override void Save(string filePath, string fileName)
        {
            base.Save(filePath, fileName);

            dataSavePath = filePath;
            dataFileName = fileName;

            SaveDataUtility.SaveData(this, fileName, filePath);
        }
        public override void Save()
        {
            base.Save();

            SaveDataUtility.SaveData(this, dataFileName, dataSavePath);
        }
    }
}
