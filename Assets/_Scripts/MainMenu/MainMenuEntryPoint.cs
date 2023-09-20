using UnityEngine;

namespace Game.Menu
{
    using Game.Menu.Pause.Exit;
    using Game.Menu.Settings;
    using UI;

    public class MainMenuEntryPoint : MonoBehaviour, IEntryPoint
    {
        [SerializeField] private MainMenu MainMenuManager;
        [SerializeField] private SettingsManager SettingsManager;
        [SerializeField] private ExitManager ExitManager;

        private UIWindowService _uiWindowService;

        private void Awake()
        {
            _uiWindowService = new UIWindowService();
            RegisterServices();
        }
        private void Start()
        {
            InitializeComponents();
        }

        private void OnDisable()
        {
            UnregisterServices();
        }

        public void InitializeComponents()
        {
            _uiWindowService.Initialize();

            MainMenuManager.Initialize(_uiWindowService);
            SettingsManager.Initialize(_uiWindowService);
            ExitManager.Initialize(_uiWindowService);
        }

        public void RegisterServices()
        {
            ServiceLocator.Register(_uiWindowService);

            ServiceLocator.Register(MainMenuManager);
        }

        public void UnregisterServices()
        {
            ServiceLocator.UnregisterAllServices();
        }
    }
}
