using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Menu.Settings
{
    using UI;
    using Save;

    public class SettingsWindow : Window
    {

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

        public override WindowID ID => WindowID.Settings;

        private SettingsSaveData _settingsSaveData => SaveManager.SettingsSaveData;
        private float _minVolume => SettingsManager.MIN_VOLUME;
        private float _maxVolume => SettingsManager.MAX_VOLUME;

        private SettingsManager _settings;

        public void Initialize(SettingsManager settings)
        {
            _settings = settings;

            InitVolumeSliders();
            InitLogToggle();
        }

        #region Init
        private void InitVolumeSliders()
        {
            // Set ranges
            // Min
            MasterSlider.minValue = _minVolume;
            MusicSlider.minValue = _minVolume;
            EffectsSlider.minValue = _minVolume;
            // Max
            MasterSlider.maxValue = _maxVolume;
            MusicSlider.maxValue = _maxVolume;
            EffectsSlider.maxValue = _maxVolume;

            // Subsciptions
            MasterSlider.onValueChanged.AddListener(MasterVolumeUpdate);
            MusicSlider.onValueChanged.AddListener(MusicVolumeUpdate);
            EffectsSlider.onValueChanged.AddListener(EffectVolumeUpdate);

            // Updating values
            MasterSlider.value = _settingsSaveData.MasterVolume;
            MusicSlider.value = _settingsSaveData.MusicVolume;
            EffectsSlider.value = _settingsSaveData.EffectsVolume;
        }
        private void InitLogToggle()
        {
            EnableLoggingToggle.onValueChanged.AddListener(LoggingEnabledUpdate);

            EnableLoggingToggle.isOn = _settingsSaveData.EnableLogs;
        }
        #endregion

        #region Events Handlers
        private void MasterVolumeUpdate(float value)
        {
            _settings.SetMasterVolume(value);
            UpdateVolumeTextVisual();
        }
        private void MusicVolumeUpdate(float value)
        {
            _settings.SetMusicVolume(value);
            UpdateVolumeTextVisual();
        }
        private void EffectVolumeUpdate(float value)
        {
            _settings.SetEffectsVolume(value);
            UpdateVolumeTextVisual();
        }

        private void LoggingEnabledUpdate(bool enabled)
        {
            _settings.SetLogsEnabled(enabled);
        }
        #endregion

        #region Visual Utils
        private void UpdateVolumeTextVisual()
        {
            MasterVolumeText.text = GetVolumeInPercent(MasterSlider.value, _maxVolume).ToString("F0") + "%";
            MusicVolumeText.text = GetVolumeInPercent(MusicSlider.value, _maxVolume).ToString("F0") + "%";
            EffectsVolumeText.text = GetVolumeInPercent(EffectsSlider.value, _maxVolume).ToString("F0") + "%";
        }

        private float GetVolumeInPercent(float volume, float maxVolume)
        {
            return (volume / maxVolume) * 100f;
        }
        #endregion
    }
}