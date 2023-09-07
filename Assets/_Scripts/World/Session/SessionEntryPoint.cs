using UnityEngine;

namespace Game
{
    using Player;
    using CraftSystem.Oven.Manager;
    using Game.Inventory;
    using Game.Enemy;
    using Game.Session;
    using Game.MainMenu.Pause;
    using Game.CraftSystem.Craft;
    using Game.SaveData;

    public class SessionEntryPoint : MonoBehaviour, IEntryPoint
    {
        [SerializeField] private UIPanelManager UIPanelManager;
        [SerializeField] private PlayerInventory PlayerInventory;
        [SerializeField] private PauseManager PauseManager;
        [Space]
        [SerializeField] private SessionManager SessionManager;
        [SerializeField] private EnemySpawner EnemySpawner;
        [SerializeField] private CraftManager CraftManager;
        [SerializeField] private OvenManager OvenManager;
        [Space]
        [SerializeField] private PlayerController Player;

        private void Awake()
        {
            RegisterServices();
        }
        private void Start()
        {
            InitializeComponents();
        }
        private void OnDisable()
        {
            UnregisterServices();
            SaveDataManager.Instance.CurrentSessionData.Save();
        }

        public void InitializeComponents()
        {
            PlayerInventory.Initialize();
            PauseManager.Initialize(UIPanelManager);

            SessionManager.Initialize(EnemySpawner, PauseManager);
            EnemySpawner.Initialize(SessionManager);
            OvenManager.Initialize(UIPanelManager, PlayerInventory);
            CraftManager.Initialize(PlayerInventory, Player);

            Player.Initialize();
        }

        public void RegisterServices()
        {
            ServiceLocator.Register(PlayerInventory);
            ServiceLocator.Register(UIPanelManager);
            ServiceLocator.Register(PauseManager);

            ServiceLocator.Register(OvenManager);
            ServiceLocator.Register(SessionManager);
            ServiceLocator.Register(EnemySpawner);
            ServiceLocator.Register(CraftManager);

            ServiceLocator.Register(Player);
        }

        public void UnregisterServices()
        {
            ServiceLocator.UnregisterAllServices();
        }
    }
}
