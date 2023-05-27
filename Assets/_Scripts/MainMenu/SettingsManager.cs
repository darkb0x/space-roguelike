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

        private const float MIN_VOLUME = 0.0001f;
        private const float MAX_VOLUME = 1f;

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

        private SettingsData settingsData => SaveDataManager.Instance.CurrentSettingsData;

        private void Start()
        {
            // min volume
            MasterSlider.minValue = MIN_VOLUME;
            MusicSlider.minValue = MIN_VOLUME;
            EffectsSlider.minValue = MIN_VOLUME;
            // max max volume
            MasterSlider.maxValue = MAX_VOLUME;
            MusicSlider.maxValue = MAX_VOLUME;
            EffectsSlider.maxValue = MAX_VOLUME;

            MasterSlider.onValueChanged.AddListener(value => SetMasterVolume(value));
            MusicSlider.onValueChanged.AddListener(value => SetMusicVolume(value));
            EffectsSlider.onValueChanged.AddListener(value => SetEffectsVolume(value));

            MasterSlider.value = settingsData.MasterVolume;
            MusicSlider.value = settingsData.MusicVolume;
            EffectsSlider.value = settingsData.EffectsVolume;

            SetMasterVolume(settingsData.MasterVolume);
            SetMusicVolume(settingsData.MusicVolume);
            SetEffectsVolume(settingsData.EffectsVolume);

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
            MasterVolumeText.text = GetVolumeInPercent(MasterSlider.value, MAX_VOLUME).ToString("F0") + "%";
            MusicVolumeText.text = GetVolumeInPercent(MusicSlider.value, MAX_VOLUME).ToString("F0") + "%";
            EffectsVolumeText.text = GetVolumeInPercent(EffectsSlider.value, MAX_VOLUME).ToString("F0") + "%";
        }

        private float GetVolumeInPercent(float volume, float maxVolume)
        {
            return (volume / maxVolume) * 100f;
        }

        public void SetMasterVolume(float value)
        {
            settingsData.MasterVolume = value;
            Mixer.audioMixer.SetFloat(MASTER_VOLUME_KEY, Mathf.Log10(value) * 30f);

            settingsData.Save();
            UpdateVolumeVisual();
        }
        public void SetMusicVolume(float value)
        {
            settingsData.MusicVolume = value;
            Mixer.audioMixer.SetFloat(MUSIC_VOLUME_KEY, Mathf.Log10(value) * 30f);

            settingsData.Save();
            UpdateVolumeVisual();
        }
        public void SetEffectsVolume(float value)
        {
            settingsData.EffectsVolume = value;
            Mixer.audioMixer.SetFloat(EFFECTS_VOLUME_KEY, Mathf.Log10(value) * 30f);

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
