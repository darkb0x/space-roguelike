using UnityEngine;
using UnityEditor;

namespace Game.SaveData
{
    [CustomEditor(typeof(SaveDataManager))]
    public class SaveDataManagerEditor : Editor
    {
        SaveDataManager dataManager;

        private void OnEnable()
        {
            dataManager = (SaveDataManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Reset Session Data"))
            {
                dataManager.CurrentSessionData.Reset();
            }
            if (GUILayout.Button("Reset Settings Data"))
            {
                dataManager.CurrentSettingsData.Reset();
            }
            if (GUILayout.Button("Reset UI Settings Data"))
            {
                dataManager.CurrentUISettingsData.Reset();
            }
            EditorGUILayout.Space();

            if(GUILayout.Button("Save All Data"))
            {
                dataManager.CurrentSessionData.Save(dataManager.savePath, SaveDataManager.SESSION_DATA_FILENAME);
                dataManager.CurrentSettingsData.Save(dataManager.savePath, SaveDataManager.SETTINGS_DATA_FILENAME);
                dataManager.CurrentUISettingsData.Save(dataManager.savePath, SaveDataManager.UI_SETTINGS_DATA_FILENAME);
            }

            GUI.enabled = true;
        }
    }
}
