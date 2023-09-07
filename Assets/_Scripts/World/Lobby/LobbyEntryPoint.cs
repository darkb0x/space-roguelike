using UnityEngine;

namespace Game.Lobby
{
    using Game.Audio;
    using Game.CraftSystem.Research;
    using Game.Lobby.Inventory;
    using Game.Lobby.Shop;
    using Game.MainMenu.MissionChoose;
    using Game.Save;
    using Player;

    public class LobbyEntryPoint : MonoBehaviour, IEntryPoint
    {
        [Header("Components")]
        [SerializeField] private UIPanelManager UIPanelManager;
        [SerializeField] private LobbyInventory LobbyInventory;
        [Space]
        [SerializeField] private ResearchManager ResearchManager;
        [SerializeField] private ShopManager ShopManager;
        [SerializeField] private MissionChooseManager MissionChooseManager;
        [Space]
        [SerializeField] private PlayerController Player;

        [Header("Other...")]
        [SerializeField] private AudioClip Music;

        private void Awake()
        {
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
            LobbyInventory.Initialize();

            ResearchManager.Initialize(LobbyInventory);
            ShopManager.Initialize(LobbyInventory);
            MissionChooseManager.Initialize();

            Player.Initialize();
        }

        public void RegisterServices()
        {
            ServiceLocator.Register(UIPanelManager);
            ServiceLocator.Register(LobbyInventory);

            ServiceLocator.Register(ResearchManager);
            ServiceLocator.Register(ShopManager);
            ServiceLocator.Register(MissionChooseManager);

            ServiceLocator.Register(Player);
        }

        public void UnregisterServices()
        {
            ServiceLocator.UnregisterAllServices();
        }
    }
}