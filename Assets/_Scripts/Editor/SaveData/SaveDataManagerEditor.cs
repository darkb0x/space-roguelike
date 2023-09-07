using UnityEngine;
using UnityEditor;
using System.IO;

namespace Game.Save
{
    [CustomEditor(typeof(SaveManager))]
    public class SaveDataManagerEditor : Editor
    {
        SaveManager dataManager;

        private void OnEnable()
        {
            dataManager = (SaveManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Reset Session Data"))
            {
                ResetSaveData(SaveManager.SessionSaveData);
            }
            if (GUILayout.Button("Reset Settings Data"))
            {
                ResetSaveData(SaveManager.SettingsSaveData);
            }
            if (GUILayout.Button("Reset UI Save Data"))
            {
                ResetSaveData(SaveManager.UISaveData);
            }
            EditorGUILayout.Space();

            if(GUILayout.Button("Save All Data"))
            {
                SaveManager.SessionSaveData.Save();
                SaveManager.SettingsSaveData.Save();
                SaveManager.UISaveData.Save();
            }

            GUI.enabled = true;
        }

        private void ResetSaveData(SaveData saveData)
        {
            saveData.Reset();
        }
    }
}
