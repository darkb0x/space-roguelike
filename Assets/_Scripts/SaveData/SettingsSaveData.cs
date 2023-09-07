namespace Game.Save
{
    [System.Serializable]
    public class SettingsSaveData : SaveData
    {
        public bool EnableLogs;

        public float MasterVolume;
        public float MusicVolume;
        public float EffectsVolume;

        public SettingsSaveData(string fileName, string filePath) : base(fileName, filePath, false)
        {
            Reset();
        }

        public override void Reset()
        {
            EnableLogs = false;

            MasterVolume = 0.5f;
            MusicVolume = 1f;
            EffectsVolume = 1f;
        }
    }
}
