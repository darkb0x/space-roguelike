using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Game.Utilities
{
    public static class SaveDataUtility
    {
        private static string dataType = "json";

        public static void SaveData(object data, string fileName, string filePath, bool encrypt = true)
        {
            if(string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("savePath is null or empty!");
                return;
            }

            string path = $"{filePath}{fileName}.{dataType}";

            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(path, json);
        }

        public static object LoadData<T>(string filePath, string fileName, object dataIfFileNotExist, bool encrypt = true)
        {
            if (string.IsNullOrEmpty(filePath) | string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("filePath or fileName is null");
                return null;
            }

            string path = $"{filePath}{fileName}.{dataType}";
            if (!File.Exists(path))
            {
                SaveData(dataIfFileNotExist, fileName, filePath, encrypt);
            }

            string json = File.ReadAllText(path);
            T data = JsonUtility.FromJson<T>(json);

            return data;
        }
    }
}
