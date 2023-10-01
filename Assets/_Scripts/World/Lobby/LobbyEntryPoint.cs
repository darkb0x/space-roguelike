using UnityEngine;

namespace Game.Lobby
{
    using Game.Audio;
    using Game.CraftSystem.Research;
    using Game.Lobby.Inventory;
    using Game.Lobby.Shop;
    using Lobby.Missions;
    using Game.Menu.Pause;
    using Game.Notifications;
    using Game.Save;
    using Game.UI;
    using Player;
    using UI.HUD;
    using SceneLoading;
    using Game.Console;

    public class LobbyEntryPoint : MonoBehaviour, IEntryPoint
    {
        [SerializeField] private HUDConfig HUDConfig;
        [SerializeField] private LoadSceneUtility LoadSceneUtility;
        [SerializeField] private CutsceneManager CutsceneManager;
        [SerializeField] private ConsoleController ConsoleController;

        [Header("Components")]
        [SerializeField] private PauseManager PauseManager;
        [SerializeField] private LobbyInventory LobbyInventory;
        [Space]
        [SerializeField] private ResearchManager ResearchManager;
        [SerializeField] private ShopManager ShopManager;
        [SerializeField] private MissionChooseManager MissionChooseManager;
        [Space]
        [SerializeField] private PlayerController Player;
        [SerializeField] private CameraController Camera;

        [Header("Other...")]
        [SerializeField] private AudioClip Music;

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
            Player.Oxygen.Disable();
            MusicManager.Instance.SetMusic(Music, true);
        }
        private void OnDisable()
        {
            UnregisterServices();
            SaveManager.SessionSaveData.Save();
        }

        public void InitializeComponents()
        {
            LoadSceneUtility.Initialize();

            _uiWindowService.Initialize();
            PauseManager.Initialize(_uiWindowService);
            LobbyInventory.Initialize(_uiWindowService);

            Player.Initialize();
            Camera.Initialize(Player);

            ResearchManager.Initialize(LobbyInventory, _uiWindowService);
            ShopManager.Initialize(LobbyInventory, _uiWindowService);
            MissionChooseManager.Initialize(_uiWindowService);

            _hudService.Initialize(_uiWindowService);
            _notificationService.Initialize(_hudService);

            CutsceneManager.Initialize(Player, Camera, _hudService);

            ConsoleController.Initialize(_uiWindowService);
        }

        public void RegisterServices()
        {
            ServiceLocator.Register(_coroutineRunner);
            ServiceLocator.Register(_uiWindowService);
            ServiceLocator.Register(PauseManager);
            ServiceLocator.Register(LobbyInventory);

            ServiceLocator.Register(Player);
            ServiceLocator.Register(Camera);

            ServiceLocator.Register(ResearchManager);
            ServiceLocator.Register(ShopManager);
            ServiceLocator.Register(MissionChooseManager);

            ServiceLocator.Register(_hudService);

            ServiceLocator.Register(CutsceneManager);

            ServiceLocator.Register(ConsoleController);
        }

        public void UnregisterServices()
        {
            ServiceLocator.UnregisterAllServices();
        }
    }
}