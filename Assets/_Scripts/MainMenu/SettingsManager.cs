using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MainMenu.Settings
{
    using SaveData;

    public class SettingsManager : MonoBehaviour
    {
        private SettingsData currentSettingsData => GameData.Instance.CurrentSettingsData;

        private void Start()
        {
            //SetTargetFPS(currentSettingsData.MaxFps);
        }

        private void SetTargetFPS(int fps)
        {
            Application.targetFrameRate = fps;
        }
    }
}
