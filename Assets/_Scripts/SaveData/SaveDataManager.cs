using UnityEngine;
using NaughtyAttributes;
using System.IO;

namespace Game.SaveData
{
    using Utilities;

    public class SaveDataManager : MonoBehaviour
    {
        public static SaveDataManager Instance;

        public const string SESSION_DATA_FILENAME = "SessionData";
        public const string SETTINGS_DATA_FILENAME = "SettingsData";
        public const string UI_SETTINGS_DATA_FILENAME = "UISettingsData";

        [ReadOnly] public string savePath;

        [field: SerializeField] public SessionData CurrentSessionData { get; private set; }
        [field: SerializeField] public SettingsData CurrentSettingsData { get; private set; }
        [field: SerializeField] public UISettingsData CurrentUISettingsData { get; private set; }

        private void Awake()
        {
            Instance = this;

            #if UNITY_EDITOR
            savePath = $"{Application.dataPath}/Editor/SaveData/";
            #else
            savePath = $"{Application.dataPath}/Save/";
            #endif

            if(!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            CurrentSessionData = SaveDataUtility.LoadDataFromJson<SessionData>(savePath, SESSION_DATA_FILENAME, true);
            CurrentSettingsData = SaveDataUtility.LoadDataFromJson<SettingsData>(savePath, SETTINGS_DATA_FILENAME);
            CurrentUISettingsData = SaveDataUtility.LoadDataFromJson<UISettingsData>(savePath, UI_SETTINGS_DATA_FILENAME);

            if(CurrentSessionData == null)
            {
                CurrentSessionData = new SessionData(savePath, SESSION_DATA_FILENAME);
                SaveDataUtility.SaveDataToJson(CurrentSessionData, SESSION_DATA_FILENAME, savePath, true);
            }
            if (CurrentSettingsData == null)
            {
                CurrentSettingsData = new SettingsData(savePath, SETTINGS_DATA_FILENAME);
                SaveDataUtility.SaveDataToJson(CurrentSettingsData, SETTINGS_DATA_FILENAME, savePath);
            }
            if (CurrentUISettingsData == null)
            {
                CurrentUISettingsData = new UISettingsData(savePath, UI_SETTINGS_DATA_FILENAME);
                SaveDataUtility.SaveDataToJson(CurrentUISettingsData, UI_SETTINGS_DATA_FILENAME, savePath);
            }
        }
    }
}
