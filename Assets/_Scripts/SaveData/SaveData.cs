using UnityEngine;

namespace Game.Save
{
    using Utilities;

    [System.Serializable]
    public abstract class SaveData
    {
        [SerializeField] protected string _filePath;
        [SerializeField] protected string _fileName;
        [SerializeField] protected bool _encrypt;

        public SaveData(string fileName, string filePath, bool encrypt)
        {
            _fileName = fileName;
            _filePath = filePath;
            _encrypt = encrypt;
        }

        public void Save()
        {
            SaveUtility.SaveDataToJson(_filePath, _fileName, this, _encrypt);
        }

        public abstract void Reset();
    }
}
