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

            #if UNITY_EDITOR
            savePath = $"{Application.dataPath}/Editor/SaveData/";
            #else
            savePath = $"{Application.dataPath}/";
            #endif

            CurrentSessionData = SaveDataUtility.LoadDataFromJson<SessionData>(savePath, SESSION_DATA_FILENAME, new SessionData(savePath, SESSION_DATA_FILENAME), true);
            CurrentSettingsData = SaveDataUtility.LoadDataFromJson<SettingsData>(savePath, SETTINGS_DATA_FILENAME, new SettingsData(savePath, SETTINGS_DATA_FILENAME));
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void ResetSessionData()
        {
            CurrentSessionData = new SessionData(savePath, SESSION_DATA_FILENAME);
            CurrentSessionData.Save();
        }
        [Button(enabledMode: EButtonEnableMode.Playmode)]
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

    [Serializable]
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
        public string PlanetPath;

        public SessionData(string savePath, string fileName)
        {
            dataSavePath = savePath;
            dataFileName = fileName;
 
            UnlockedCraftPaths = new List<string>();

            MainInventory = new Inventory();
            LobbyInventory = new Inventory();

            Money = 0;

            PlanetPath = "";
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

        #region
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
            base.Save(filePath, fileName);

            dataSavePath = filePath;
            dataFileName = fileName;

            SaveDataUtility.SaveDataToJson(this, fileName, filePath, true);

        }
        public override void Save()
        {
            base.Save();

            SaveDataUtility.SaveDataToJson(this, dataFileName, dataSavePath, true);
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
                Items.Add(new Item() { Path = data.Item.AssetPath, Amount = data.Amount } );
            }
            public void SetItem(ItemData data)
            {
                foreach (var item in Items)
                {
                    if(item.Path == data.Item.AssetPath)
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

    [Serializable]
    public class SettingsData : Data
    {
        public float MasterVolume;
        public float MusicVolume;
        public float EffectsVolume;

        public SettingsData(string savePath, string fileName)
        {
            dataSavePath = savePath;
            dataFileName = fileName;

            MasterVolume = 0f;
            MusicVolume = 0f;
            EffectsVolume = 0f;
        }

        public override void Save(string filePath, string fileName)
        {
            base.Save(filePath, fileName);

            dataSavePath = filePath;
            dataFileName = fileName;

            SaveDataUtility.SaveDataToJson(this, dataFileName, dataSavePath);
        }
        public override void Save()
        {
            base.Save();

            SaveDataUtility.SaveDataToJson(this, dataFileName, dataSavePath);
        }
    }
}
