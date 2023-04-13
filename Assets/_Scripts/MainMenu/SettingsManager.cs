using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

namespace Game.MainMenu.Settings
{
    using SaveData;

    public class SettingsManager : MonoBehaviour
    {
        private const string MASTER_VOLUME_KEY = "MasterVolume";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string EFFECTS_VOLUME_KEY = "EffectsVolume";

        private const float MIN_VOLUME = -80f;
        private const float MAX_VOLUME = 0f;

        [SerializeField] private GameObject[] PanelsToDisable;
        [SerializeField] private GameObject MainPanel;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixerGroup Mixer;
       
        [Header("Volume Sliders")]
        [SerializeField] private Slider MasterSlider;
        [SerializeField] private TextMeshProUGUI MasterVolumeText;
        [Space]
        [SerializeField] private Slider MusicSlider;
        [SerializeField] private TextMeshProUGUI MusicVolumeText;
        [Space]
        [SerializeField] private Slider EffectsSlider;
        [SerializeField] private TextMeshProUGUI EffectsVolumeText;

        [Header("Logging Toggle")]
        [SerializeField] private Toggle EnableLoggingToggle;

        private SettingsData settingsData => GameData.Instance.CurrentSettingsData;

        private void Start()
        {
            // Volume
            MasterSlider.minValue = MIN_VOLUME;
            MusicSlider.minValue = MIN_VOLUME;
            EffectsSlider.minValue = MIN_VOLUME;

            MasterSlider.maxValue = MAX_VOLUME;
            MusicSlider.maxValue = MAX_VOLUME;
            EffectsSlider.maxValue = MAX_VOLUME;

            MasterSlider.value = settingsData.MasterVolume;
            MusicSlider.value = settingsData.MusicVolume;
            EffectsSlider.value = settingsData.EffectsVolume;

            // Logs
            EnableLoggingToggle.isOn = settingsData.EnableLogs;

            UpdateVolumeVisual();
        }

        public void OpenPanel()
        {
            MainPanel.SetActive(true);

            foreach (var panel in PanelsToDisable)
            {
                panel.SetActive(false);
            }
        }
        public void ClosePanel()
        {
            MainPanel.SetActive(false);

            foreach (var panel in PanelsToDisable)
            {
                panel.SetActive(true);
            }
        }

        #region Volume
        private void UpdateVolumeVisual()
        {
            MasterVolumeText.text = GetVolumeInPercent(MasterSlider.value).ToString("F0") + "%";
            MusicVolumeText.text = GetVolumeInPercent(MusicSlider.value).ToString("F0") + "%";
            EffectsVolumeText.text = GetVolumeInPercent(EffectsSlider.value).ToString("F0") + "%";
        }

        private float GetVolumeInPercent(float volume)
        {
            return Mathf.InverseLerp(MIN_VOLUME, MAX_VOLUME, volume) * 100f;
        }

        public void SetMasterVolume(float value)
        {
            settingsData.MasterVolume = value;
            Mixer.audioMixer.SetFloat(MASTER_VOLUME_KEY, value);

            settingsData.Save();
            UpdateVolumeVisual();
        }
        public void SetMusicVolume(float value)
        {
            settingsData.MusicVolume = value;
            Mixer.audioMixer.SetFloat(MUSIC_VOLUME_KEY, value);

            settingsData.Save();
            UpdateVolumeVisual();
        }
        public void SetEffectsVolume(float value)
        {
            settingsData.EffectsVolume = value;
            Mixer.audioMixer.SetFloat(EFFECTS_VOLUME_KEY, value);

            settingsData.Save();
            UpdateVolumeVisual();
        }
        #endregion

        #region Logging
        public void EnableLogs(bool enabled)
        {
            settingsData.EnableLogs = enabled;

            settingsData.Save();
        }
        #endregion
    }
}
