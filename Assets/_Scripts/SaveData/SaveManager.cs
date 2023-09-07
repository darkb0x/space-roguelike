using UnityEngine;
using NaughtyAttributes;
using System.IO;

namespace Game.Save
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using Utilities;

    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance;

        private const string SESSION_SAVEDATA_FILENAME = "SessionSaveData";
        private const string SETTINGS_SAVEDATA_FILENAME = "SettingsSaveData";
        private const string UI_SAVEDATA_FILENAME = "UISaveData";

        public static SessionSaveData SessionSaveData => Instance.m_SessionSaveData;
        public static SettingsSaveData SettingsSaveData => Instance.m_SettingsSaveData;
        public static UISaveData UISaveData => Instance.m_UISaveData;

        [ReadOnly] public string savePath;

        [SerializeField] private SessionSaveData m_SessionSaveData;
        [SerializeField] private SettingsSaveData m_SettingsSaveData;
        [SerializeField] private UISaveData m_UISaveData;

        public void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            #if UNITY_EDITOR
            savePath = $"{Application.dataPath}/Editor/SaveData/";
            #else
            savePath = $"{Application.dataPath}/Save/";
            #endif

            if(!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            m_SessionSaveData = SaveUtility.LoadDataFromJson<SessionSaveData>(savePath, SESSION_SAVEDATA_FILENAME);
            m_SettingsSaveData = SaveUtility.LoadDataFromJson<SettingsSaveData>(savePath, SETTINGS_SAVEDATA_FILENAME);
            m_UISaveData = SaveUtility.LoadDataFromJson<UISaveData>(savePath, UI_SAVEDATA_FILENAME);

            if(m_SessionSaveData == null)
            {
                m_SessionSaveData = new SessionSaveData(SESSION_SAVEDATA_FILENAME, savePath);
                m_SessionSaveData.Save();
            }
            if (m_SettingsSaveData == null)
            {
                m_SettingsSaveData = new SettingsSaveData(SETTINGS_SAVEDATA_FILENAME, savePath);
                m_SettingsSaveData.Save();
            }
            if (m_UISaveData == null)
            {
                m_UISaveData = new UISaveData(UI_SAVEDATA_FILENAME, savePath);
                m_UISaveData.Save();
            }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}
