namespace Game.Save
{
    [System.Serializable]
    public class UISaveData : SaveData
    {
        public float CameraZoom;
        public bool EnableTimeline;
        public bool EnableNotifications;

        public UISaveData(string fileName, string filePath) : base(fileName, filePath, false)
        {
            Reset();
        }

        public override void Reset()
        {
            CameraZoom = 6f;
            EnableTimeline = true;
            EnableNotifications = true;
        }
    }
}
