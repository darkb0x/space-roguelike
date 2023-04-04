using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Game.Utilities
{
    public static class SaveDataUtility
    {
        private static string jsonDataType = "json";
        private static string binaryDataType = "save";

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

        public static T LoadDataFromJson<T>(string filePath, string fileName, object dataIfFileNotExist, bool ecrypt = false)
        {
            if (string.IsNullOrEmpty(filePath) | string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("filePath or fileName is null");
                return default(T);
            }

            string path = $"{filePath}{fileName}.{jsonDataType}";
            if (!File.Exists(path))
            {
                SaveDataToJson(dataIfFileNotExist, fileName, filePath, ecrypt);
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

        // Must be fixed but I'm lazy
        public static void SaveDataToBinary(object data, string fileName, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("savePath is null or empty!");
                return;
            }

            string path = $"{filePath}{fileName}.{binaryDataType}";

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(stream, data);
            }
        }

        // Must be fixed but I'm lazy 2
        public static T LoadDataFromBinary<T>(string filePath, string fileName, object defaultData)
        {
            if (string.IsNullOrEmpty(filePath) | string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("filePath or fileName is null");
                return default(T);
            }

            string path = $"{filePath}{fileName}.{binaryDataType}";
            if (!File.Exists(path))
            {
                SaveDataToBinary(defaultData, fileName, filePath);
            }

            BinaryFormatter formatter = new BinaryFormatter();
            using(FileStream stream = new FileStream(path, FileMode.Open))
            {
                return (T)formatter.Deserialize(stream);
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
