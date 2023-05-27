namespace Game.SaveData
{
    [System.Serializable]
    public abstract class Data
    {
        public string dataSavePath;
        public string dataFileName;

        public abstract void Save(string filePath, string fileName);
        public abstract void Save();

        public virtual void Reset()
        {
            // reset logic
            Save();
        }
    }
}
