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

    public class LobbyEntryPoint : MonoBehaviour, IEntryPoint
    {
        [SerializeField] private HUDConfig HUDConfig;

        [Header("Components")]
        [SerializeField] private PauseManager PauseManager;
        [SerializeField] private LobbyInventory LobbyInventory;
        [Space]
        [SerializeField] private ResearchManager ResearchManager;
        [SerializeField] private ShopManager ShopManager;
        [SerializeField] private MissionChooseManager MissionChooseManager;
        [Space]
        [SerializeField] private PlayerController Player;

        [Header("Other...")]
        [SerializeField] private AudioClip Music;

        private UIWindowService _uiWindowService;
        private HUDService _hudService;
        private NotificationService _notificationService;

        private void Awake()
        {
            _uiWindowService = new UIWindowService();
            _hudService = new HUDService(HUDConfig);
            _notificationService = new NotificationService();
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
            _uiWindowService.Initialize();
            PauseManager.Initialize(_uiWindowService);
            LobbyInventory.Initialize(_uiWindowService);

            ResearchManager.Initialize(LobbyInventory, _uiWindowService);
            ShopManager.Initialize(LobbyInventory, _uiWindowService);
            MissionChooseManager.Initialize(_uiWindowService);

            Player.Initialize();

            _hudService.Initialize(_uiWindowService);
            _notificationService.Initialize(_hudService);
        }

        public void RegisterServices()
        {
            ServiceLocator.Register(_uiWindowService);
            ServiceLocator.Register(PauseManager);
            ServiceLocator.Register(LobbyInventory);

            ServiceLocator.Register(ResearchManager);
            ServiceLocator.Register(ShopManager);
            ServiceLocator.Register(MissionChooseManager);

            ServiceLocator.Register(Player);

            ServiceLocator.Register(_hudService);
        }

        public void UnregisterServices()
        {
            ServiceLocator.UnregisterAllServices();
        }
    }
}