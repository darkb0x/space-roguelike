using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SaveData
{
    using Utilities;

    [System.Serializable]
    public class SettingsData : Data
    {
        public bool EnableLogs;

        public float MasterVolume;
        public float MusicVolume;
        public float EffectsVolume;

        public SettingsData(string savePath, string fileName)
        {
            dataSavePath = savePath;
            dataFileName = fileName;

            Reset();
        }

        public override void Save(string filePath, string fileName)
        {
            dataSavePath = filePath;
            dataFileName = fileName;

            Save();
        }
        public override void Save()
        {
            SaveUtility.SaveDataToJson(dataSavePath, dataFileName, this);
        }
        public override void Reset()
        {
            EnableLogs = false;

            MasterVolume = 0.5f;
            MusicVolume = 1f;
            EffectsVolume = 1f;

            base.Reset();
        }
    }
}
