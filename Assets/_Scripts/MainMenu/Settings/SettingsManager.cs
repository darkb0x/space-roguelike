using UnityEngine;
using UnityEngine.Audio;

namespace Game.Menu.Settings
{
    using UI;
    using Save;

    public class SettingsManager : MonoBehaviour, IEntryComponent<UIWindowService>
    {
        public const WindowID SETTINGS_WINDOW_ID = WindowID.Settings;
        public const string MASTER_VOLUME_KEY = "MasterVolume";
        public const string MUSIC_VOLUME_KEY = "MusicVolume";
        public const string EFFECTS_VOLUME_KEY = "EffectsVolume";

        public const float MIN_VOLUME = 0.0001f;
        public const float MAX_VOLUME = 1f;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixerGroup Mixer;

        private SettingsSaveData settingsData => SaveManager.SettingsSaveData;

        public void Initialize(UIWindowService windowService)
        {
            windowService.RegisterWindow<SettingsWindow>(SETTINGS_WINDOW_ID).Initialize(this);
        }

        public void SetMasterVolume(float value)
        {
            settingsData.MasterVolume = value;
            Mixer.audioMixer.SetFloat(MASTER_VOLUME_KEY, Mathf.Log10(value) * 30f);

            settingsData.Save();
        }
        public void SetMusicVolume(float value)
        {
            settingsData.MusicVolume = value;
            Mixer.audioMixer.SetFloat(MUSIC_VOLUME_KEY, Mathf.Log10(value) * 30f);

            settingsData.Save();
        }
        public void SetEffectsVolume(float value)
        {
            settingsData.EffectsVolume = value;
            Mixer.audioMixer.SetFloat(EFFECTS_VOLUME_KEY, Mathf.Log10(value) * 30f);

            settingsData.Save();
        }

        public void SetLogsEnabled(bool enabled)
        {
            settingsData.EnableLogs = enabled;

            settingsData.Save();
        }
    }
}
