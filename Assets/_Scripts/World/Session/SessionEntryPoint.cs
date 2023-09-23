using UnityEngine;

namespace Game.Session
{
    using Player;
    using CraftSystem.Oven;
    using UI;
    using UI.HUD;
    using Game.Enemy;
    using Game.Menu.Pause;
    using Game.CraftSystem.Craft;
    using Game.Save;
    using Game.Notifications;

    public class SessionEntryPoint : MonoBehaviour, IEntryPoint
    {
        [SerializeField] private HUDConfig HUDConfig;

        [Header("Components")]
        [SerializeField] private PlayerInventory PlayerInventory;
        [SerializeField] private PauseManager PauseManager;
        [Space]
        [SerializeField] private SessionManager SessionManager;
        [SerializeField] private EnemySpawner EnemySpawner;
        [SerializeField] private CraftManager CraftManager;
        [SerializeField] private OvenManager OvenManager;
        [Space]
        [SerializeField] private PlayerController Player;
        [SerializeField] private CameraController Camera;

        private UIWindowService _uiWindowService;
        private HUDService _hudService;
        private NotificationService _notificationService;
        private CoroutineRunner _coroutineRunner;

        private void Awake()
        {
            _uiWindowService = new UIWindowService();
            _hudService = new HUDService(HUDConfig);
            _notificationService = new NotificationService();
            _coroutineRunner = Instantiate(Resources.Load<CoroutineRunner>(AssetPathConstants.COROUTINE_RUNNER));
            RegisterServices();
        }
        private void Start()
        {
            InitializeComponents();
        }
        private void OnDisable()
        {
            UnregisterServices();
            SaveManager.SessionSaveData.Save();
        }

        public void InitializeComponents()
        {
            _uiWindowService.Initialize();
            PlayerInventory.Initialize();
            PauseManager.Initialize(_uiWindowService);

            SessionManager.Initialize(EnemySpawner, PauseManager);
            EnemySpawner.Initialize(SessionManager);
            OvenManager.Initialize(_uiWindowService, PlayerInventory);
            CraftManager.Initialize(PlayerInventory, Player, _uiWindowService);

            Player.Initialize();
            Camera.Initialize(Player);

            _hudService.Initialize(_uiWindowService);
            _notificationService.Initialize(_hudService);
        }

        public void RegisterServices()
        {
            ServiceLocator.Register(_coroutineRunner);
            ServiceLocator.Register(_uiWindowService);
            ServiceLocator.Register(PlayerInventory);
            ServiceLocator.Register(PauseManager);

            ServiceLocator.Register(OvenManager);
            ServiceLocator.Register(SessionManager);
            ServiceLocator.Register(EnemySpawner);
            ServiceLocator.Register(CraftManager);

            ServiceLocator.Register(Player);
            ServiceLocator.Register(Camera);

            ServiceLocator.Register(_hudService);
        }

        public void UnregisterServices()
        {
            ServiceLocator.UnregisterAllServices();
        }
    }
}
