using UnityEngine;

namespace Game.SaveData
{
    using Utilities;

    public class UISettingsData : Data
    {
        public float CameraZoom;
        public bool EnableTimeline;

        public UISettingsData(string savePath, string fileName)
        {
            dataSavePath = savePath;
            dataFileName = fileName;

            Reset();
        }

        public override void Save(string filePath, string fileName)
        {
            dataSavePath = filePath;
            dataFileName = fileName;

            SaveDataUtility.SaveDataToJson(this, dataFileName, dataSavePath);
        }
        public override void Save()
        {
            SaveDataUtility.SaveDataToJson(this, dataFileName, dataSavePath);
        }
        public override void Reset()
        {
            CameraZoom = 6f;
            EnableTimeline = true;

            base.Reset();
        }
    }
}
