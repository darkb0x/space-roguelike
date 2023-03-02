using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace Game.SaveData
{
    using Player.Inventory;
    using Utilities;

    public class GameData : MonoBehaviour
    {
        public static GameData Instance;

        public const string SESSION_DATA_FILENAME = "SessionData";
        public const string SETTINGS_DATA_FILENAME = "SettingsData";

        public SessionData CurrentSessionData;
        public SettingsData CurrentSettingsData;

        private string savePath;

        private void Awake()
        {
            Instance = this;

            savePath = $"{Application.dataPath}/";
            
            CurrentSessionData = (SessionData)SaveDataUtility.LoadData<SessionData>(savePath, SESSION_DATA_FILENAME, new SessionData());
            CurrentSettingsData = (SettingsData)SaveDataUtility.LoadData<SettingsData>(savePath, SETTINGS_DATA_FILENAME, new SettingsData());
        }

        public void ResetAllData()
        {
            CurrentSessionData = new SessionData();
            CurrentSettingsData = new SettingsData();

            CurrentSessionData.Save(savePath, SESSION_DATA_FILENAME);
            CurrentSettingsData.Save(savePath, SETTINGS_DATA_FILENAME);
        }

        [Button]
        public void Save()
        {
            CurrentSessionData.Save(savePath, SESSION_DATA_FILENAME);
            CurrentSettingsData.Save(savePath, SETTINGS_DATA_FILENAME);
        }

        public abstract class Data
        {
            public virtual void Save(string filePath, string fileName) { }
        }

        [Serializable]
        public class SessionData : Data
        {
            // Crafts
            public List<string> UnlockedCraftPaths;

            // Inventory
            public SerializableDictionary<string, int> Items;
            public int Money;

            public SessionData()
            {
                UnlockedCraftPaths = new List<string>();

                Items = new SerializableDictionary<string, int>();
                Money = 0;
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
            public ItemData GetItem(string path)
            {
                foreach (var item in Items.Keys)
                {
                    if(item == path)
                    {
                        InventoryItem inventoryItem = Resources.Load<InventoryItem>(item);
                        ItemData data = new ItemData()
                        {
                            Item = inventoryItem,
                            Amount = Items[item]
                        };
                        return data;
                    }
                }
                return null;
            }

            public override void Save(string filePath, string fileName)
            {
                base.Save(filePath, fileName);
                SaveDataUtility.SaveData(this, fileName, filePath);
            }

            [Serializable]
            public class SerializableKeyValuePair
            {
                public string Key;
                public int Value;

                public SerializableKeyValuePair(string key, int value)
                {
                    this.Key = key;
                    this.Value = value;
                }
            }
        }

        [Serializable]
        public class SettingsData : Data
        {
            public int MaxFps = 60;

            public SettingsData()
            {
                MaxFps = 60;
            }

            public override void Save(string filePath, string fileName)
            {
                base.Save(filePath, fileName);
                SaveDataUtility.SaveData(this, fileName, filePath);
            }
        }
    }
}
