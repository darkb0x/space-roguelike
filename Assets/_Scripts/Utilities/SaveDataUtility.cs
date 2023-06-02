using UnityEngine;
using System.IO;

namespace Game.Utilities
{
    public static class SaveDataUtility
    {
        private static string jsonDataType = "json";

        private static string encryptKey = "217702026";

        public static void SaveDataToJson(object data, string fileName, string filePath, bool encrypt = false)
        {
            if(string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("savePath is null or empty!");
                return;
            }

            string path = $"{filePath}{fileName}.{jsonDataType}";

            string json = JsonUtility.ToJson(data, true);

            if(encrypt)
            {
                File.WriteAllText(path, EncryptDecrypt(json));
            }
            else
            {
                File.WriteAllText(path, json);
            }
        }

        public static T LoadDataFromJson<T>(string filePath, string fileName, bool ecrypt = false)
        {
            if (string.IsNullOrEmpty(filePath) | string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("filePath or fileName is null");
                return default(T);
            }

            string path = $"{filePath}{fileName}.{jsonDataType}";
            if (!File.Exists(path))
            {
                Debug.LogWarning(path + " is not exist.");
                return default(T);
            }

            string json = File.ReadAllText(path);

            if(ecrypt)
            {
                return JsonUtility.FromJson<T>(EncryptDecrypt(json));
            }
            else
            {
                return JsonUtility.FromJson<T>(json);
            }
        }

        public static string EncryptDecrypt(string data)
        {
            string result = "";

            for (int i = 0; i < data.Length; i++)
            {
                result += (char)(data[i] ^ encryptKey[i % encryptKey.Length]);
            }

            return result;
        }
    }
}
