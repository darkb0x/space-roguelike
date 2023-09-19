using UnityEngine;
using UnityEngine.UI;

namespace Game.Menu.Pause
{
    using UI;
    using Settings;
    using Input;
    using Exit;

    public class PauseWindow : Window
    {
        public const WindowID SETTINGS_WINDOW_ID = SettingsManager.SETTINGS_WINDOW_ID;
        public const WindowID EXIT_WINDOW_ID = ExitManager.EXIT_WINDOW_ID;

        public override WindowID ID => PauseManager.PAUSE_WINDOW_ID;

        [SerializeField] private SettingsManager Settings;
        [SerializeField] private ExitManager Exit;
        [Space]
        [SerializeField] private Button SettingsButton;
        [SerializeField] private Button ReturnButton;
        [SerializeField] private Button ExitButton;

        private UIInputHandler _input => InputManager.UIInputHandler;

        private SettingsWindow _settingsSubWindow;
        private ExitWindow _exitSubWindow;

        public override void Initialize(UIWindowService windowService)
        {
            base.Initialize(windowService);

            // Register settings as sub window
            _settingsSubWindow = _uiWindowService.RegisterWindow<SettingsWindow>(SETTINGS_WINDOW_ID, this);
            _settingsSubWindow.Initialize(Settings);
            AddSubWindow(_settingsSubWindow);

            _exitSubWindow = _uiWindowService.RegisterWindow<ExitWindow>(EXIT_WINDOW_ID, this);
            _exitSubWindow.Initialize(Exit);
            AddSubWindow(_exitSubWindow);

            // Subscribing to events
            InitButtons();
        }

        private void InitButtons()
        {
            SettingsButton.onClick.AddListener(OpenSettings);
            ReturnButton.onClick.AddListener(() => Close());
            ExitButton.onClick.AddListener(OpenExit);
        }

        public void OpenSettings()
        {
            OpenSubWindow(SETTINGS_WINDOW_ID);
        }
        public void OpenExit()
        {
            OpenSubWindow(EXIT_WINDOW_ID);
        }
    }
}
