using UnityEngine;

namespace Game.SaveData
{
    using Utilities;

    public class UISettingsData : Data
    {
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
            base.Reset();
        }
    }
}
