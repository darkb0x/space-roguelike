using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using NaughtyAttributes;

namespace Game.Utilities.CheatPanel
{
    using Player;
    using Player.Pick;
    using Player.Inventory;
    using CraftSystem.ScriptableObjects;
    using Turret;
    using Drone;
    using Enemy;
    using Drill;

    public class CheatPanel : MonoBehaviour
    {
        [SerializeField] private GameObject Visual;
        [Space]
        [SerializeField] private PlayerController PlayerController;
        [Space]

        [BoxGroup("Player"), SerializeField] private PlayerCheatPanel PlayerCheats;
        [BoxGroup("Player"), SerializeField] private GameObject PlayerCheatsPanel;
        [BoxGroup("Player"), SerializeField] private bool InitializePlayerCheats = true;

        //[BoxGroup("Crafts"), SerializeField] private CraftsCheatPanel CraftCheats;
        [BoxGroup("Crafts"), SerializeField] private GameObject CraftCheatsPanel;
        [BoxGroup("Crafts"), SerializeField] private bool InitializeCraftCheats = true;

        [BoxGroup("Enemy"), SerializeField] private EnemyCheatPanel EnemyCheats;
        [BoxGroup("Enemy"), SerializeField] private GameObject EnemyCheatsPanel;
        [BoxGroup("Enemy"), SerializeField] private bool InitializeEnemyCheats = true;

        private bool isOpened;

        private void Start()
        {
            GameInput.InputActions.Player.CheatPanel.performed += OpenCloseCheatPanel;
            GameInput.InputActions.UI.CheatPanel.performed += OpenCloseCheatPanel;

            isOpened = false;
            Visual.SetActive(false);

            PlayerCheatsPanel.SetActive(false);
            CraftCheatsPanel.SetActive(false);
            EnemyCheatsPanel.SetActive(false);

            if (InitializePlayerCheats)
            {
                PlayerCheats.Initialize(PlayerController);
                PlayerCheatsPanel.SetActive(true);
            }
            if(InitializeCraftCheats)
            {
                //CraftCheats.Initialize(PlayerController);
                CraftCheatsPanel.SetActive(true);
            }
            if(InitializeEnemyCheats)
            {
                EnemyCheats.Initialize(PlayerController);
                EnemyCheatsPanel.SetActive(true);
            }
        }

        private void OnDisable()
        {
            GameInput.InputActions.Player.CheatPanel.performed -= OpenCloseCheatPanel;
            GameInput.InputActions.UI.CheatPanel.performed -= OpenCloseCheatPanel;
        }

        private void OpenCloseCheatPanel(InputAction.CallbackContext callback)
        {
            isOpened = !isOpened;

            Visual.SetActive(isOpened);
        }

        public static class Utilities
        {
            public static void CreateButton_Icon(string name, Vector2 size, Sprite icon, Transform parent, UnityAction onClickEvent)
            {
                GameObject buttonObject = new GameObject();
                RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
                Image buttonVisual = buttonObject.AddComponent<Image>();
                Button buttonComponent = buttonObject.AddComponent<Button>();

                buttonObject.name = name;
                buttonRect.sizeDelta = size;
                buttonVisual.sprite = icon;
                buttonVisual.preserveAspect = true;
                buttonComponent.onClick.AddListener(onClickEvent);

                buttonObject.transform.SetParent(parent);
                buttonObject.transform.localScale = Vector3.one;
            }
        }
    }

    [System.Serializable]
    public class PlayerCheatPanel
    {
        [SerializeField] private Toggle DoHealthCycleToggle;
        [SerializeField] private Toggle DoOxygenCycleToggle;
        [Space]
        [SerializeField] private Button GiveMoneyButton;
        [SerializeField] private TMP_InputField GiveMoneyInputField;
        [Space]
        [SerializeField] private Button GiveItemButton;
        [SerializeField] private TMP_InputField GiveItemInputField;
        [SerializeField] private Button ChooseItemButton;
        [SerializeField] private Image ChoosedItemVisual;
        [Space]
        [SerializeField] private Button HurtPlayerButton;

        [Header("Item Selection Panel")]
        [SerializeField] private List<InventoryItem> Items = new List<InventoryItem>();
        [SerializeField] private Transform ItemVisualsParent;
        [SerializeField] private GameObject ItemListPanel;

        private InventoryItem choosedItem;
        private PlayerController player;
        private bool itemSelectionPanelIsOpened;

        private PlayerInventory PlayerInventory;

        public void Initialize(PlayerController player)
        {
            PlayerInventory = Singleton.Get<PlayerInventory>();

            this.player = player;
            itemSelectionPanelIsOpened = false;

            DoHealthCycleToggle.isOn = player.DoHealthCycle;
            DoHealthCycleToggle.onValueChanged.AddListener(value => { EnableHealthCycle(value); });
            DoOxygenCycleToggle.isOn = player.DoOxygenCycle;
            DoOxygenCycleToggle.onValueChanged.AddListener(value => { EnableOxygenCycle(value); });

            GiveMoneyButton.onClick.AddListener(() => GiveMoney(GiveMoneyInputField.text));

            GiveItemButton.onClick.AddListener(() => GiveItem(GiveItemInputField.text));
            ChooseItemButton.onClick.AddListener(() => 
            {
                itemSelectionPanelIsOpened = !itemSelectionPanelIsOpened;

                ItemListPanel.SetActive(itemSelectionPanelIsOpened);
            });
            ChoosedItemVisual.color = new Color(1, 1, 1, 0);

            HurtPlayerButton.onClick.AddListener(() => { player.TakeDamage(); });

            InitializeItemSelectioPanel();
        }
        private void InitializeItemSelectioPanel()
        {
            ItemListPanel.SetActive(false);

            Vector2 buttonSize = new Vector2(100f, 100f);

            for (int i = 0; i < Items.Count; i++)
            {
                InventoryItem item = Items[i];

                CheatPanel.Utilities.CreateButton_Icon($"Item: {item.ItemName} ({i})", buttonSize, item.Icon, ItemVisualsParent, () =>
                {                   
                    choosedItem = item;
                    ChoosedItemVisual.sprite = choosedItem.Icon;
                    ChoosedItemVisual.color = new Color(1, 1, 1, 1);

                    itemSelectionPanelIsOpened = false;
                    ItemListPanel.SetActive(itemSelectionPanelIsOpened);
                });
            }
        }

        private void EnableHealthCycle(bool enabled)
        {
            player.DoHealthCycle = enabled;
            LogUtility.WriteLog($"Cheats. Player.DoHealthCycle = {enabled}");
        }
        private void EnableOxygenCycle(bool enabled)
        {
            player.DoOxygenCycle = enabled;
            LogUtility.WriteLog($"Cheats. Player.DoOxygenCycle = {enabled}");
        }

        private void GiveMoney(string amountTextField)
        {
            int amount = 0;
            foreach (var symbol in amountTextField)
            {
                if(char.IsLetter(symbol))
                {
                    Debug.Log($"Cheats. Can't add: {amountTextField} to money. Field have a letters");
                    return;
                }
            }
            amount = int.Parse(amountTextField);

            PlayerInventory.money += amount;
            LogUtility.WriteLog($"Cheats. PlayerInventory.money + {amount}");
        }
        private void GiveItem(string amountTextField)
        {
            if(choosedItem == null)
            {
                Debug.Log($"Cheats. Any item hadn't be choosed. I can't add it to inventory!");
                return;
            }

            int amount = 0;
            foreach (var symbol in amountTextField)
            {
                if (char.IsLetter(symbol))
                {
                    Debug.Log($"Cheats. Can't add: '{amountTextField}' to items. Field have a letters");
                    return;
                }
            }
            amount = int.Parse(amountTextField);

            PlayerInventory.AddItem(choosedItem, amount);
            LogUtility.WriteLog($"Cheats. PlayerInventory.AddItem({choosedItem.ItemName}, {amount})");
        }
    }

    /*
    [System.Serializable]
    public class CraftsCheatPanel
    {
        [SerializeField] private Button SpawnObjectButton;
        [SerializeField] private Button SelectBuildButton;
        [SerializeField] private Image SelectedCraftVisual;
        [Space]
        [SerializeField] private List<CSCraftContainerSO> CraftsTrees = new List<CSCraftContainerSO>();
        [SerializeField] private List<CSCraftSO> CraftsList = new List<CSCraftSO>();
        [Space]
        [SerializeField] private Transform BuildVisualsParent;
        [SerializeField] private GameObject BuildListPanel;

        private CSCraftSO selectedCraft;
        private PlayerPickObjects PlayerPickObjects;
        private PlayerController player;
        private bool buildSelectionIsOpened;

        public void Initialize(PlayerController player)
        {
            this.player = player;
            PlayerPickObjects = player.GetComponent<PlayerPickObjects>();
            buildSelectionIsOpened = false;
            BuildListPanel.SetActive(false);

            SelectedCraftVisual.color = new Color(1, 1, 1, 0);

            foreach (var tree in CraftsTrees)
            {
                for (int i = tree.Nodes.Count - 1; i >= 0; i--)
                {
                    CraftsList.Insert(0, tree.Nodes[i]);
                }
            }

            SpawnObjectButton.onClick.AddListener(() => { SpawnObject(selectedCraft); });
            SelectBuildButton.onClick.AddListener(() => 
            {
                buildSelectionIsOpened = !buildSelectionIsOpened;

                BuildListPanel.SetActive(buildSelectionIsOpened);
            });

            InitializeCraftSelectionPanel();
        }

        private void InitializeCraftSelectionPanel()
        {
            Vector2 buttonSize = new Vector2(100f, 100f);
            for (int i = 0; i < CraftsList.Count; i++)
            {
                CSCraftSO item = CraftsList[i];

                CheatPanel.Utilities.CreateButton_Icon($"Build: {item.ObjectPrefab.name} ({i})", buttonSize, item.IconSprite, BuildVisualsParent, () =>
                {
                    selectedCraft = item;

                    buildSelectionIsOpened = false;
                    BuildListPanel.SetActive(false);

                    SelectedCraftVisual.color = new Color(1, 1, 1, 1);
                    SelectedCraftVisual.sprite = selectedCraft.IconSprite;
                });
            }
        }

        private void SpawnObject(CSCraftSO craft)
        {
            if(craft == null)
            {
                Debug.Log($"Cheats. Craft hadn't been choosed");
                return;
            }
            if(craft.ObjectPrefab == null)
            {
                Debug.Log($"Cheats. Craft haven't a build prefab");
                return;
            }

            GameObject buildPrefab = craft.ObjectPrefab;
            if (PlayerPickObjects.HaveObject)
            {
                Debug.Log($"Cheats. Can't give build({buildPrefab.name}) to player! Player already have a build on his hands. ({PlayerPickObjects.pickedGameObject.name})");
                return;
            }

            GameObject craftedObj = Object.Instantiate(buildPrefab, PlayerPickObjects.transform.position, Quaternion.identity);

            if (craftedObj.TryGetComponent(out Turret turret))
            {
                turret.Initialize(player);
            }
            else if (craftedObj.TryGetComponent(out DroneAI drone))
            {
                drone.Initialize(PlayerPickObjects.GetComponent<PlayerDronesController>());
            }
            else if(craftedObj.TryGetComponent(out Drill drill))
            {
                drill.Initialize();
            }

            LogUtility.WriteLog($"Cheats. Given a build '{buildPrefab.name}', for player");
        }
    }
    */

    [System.Serializable]
    public class EnemyCheatPanel
    {
        [SerializeField] private Toggle DoSpawnEnemiesToogle;
        [Space]
        [SerializeField] private Button SpawnEnemyButton;
        [SerializeField] private Button EnemyEditButton;
        [Space]
        [SerializeField] private Button StartWaveButton;
        [SerializeField] private Button KillAllEnemiesButton;
        [Space]
        [SerializeField] private EnemyData DefaultEnemyData;

        [Header("Enemy Edit")]
        [SerializeField] private GameObject EnemyEditPanel;
        [SerializeField] private EnemySpawnData CurrentEnemySpawnData;

        private PlayerController player;
        private int defaultEnemyMaxAmount;
        private string enemyDataPath = "Enemy/";
        private bool enemyEditPanelIsOpned;

        private EnemySpawner EnemySpawner;

        public void Initialize(PlayerController player)
        {
            EnemySpawner = Singleton.Get<EnemySpawner>();

            this.player = player;
            defaultEnemyMaxAmount = EnemySpawner.EnemyMaxAmount;
            enemyEditPanelIsOpned = false;
            EnemyEditPanel.SetActive(false);

            CurrentEnemySpawnData.Initialize(DefaultEnemyData, false, Resources.LoadAll<EnemyData>(enemyDataPath));

            DoSpawnEnemiesToogle.isOn = !(EnemySpawner.EnemyMaxAmount == 0);
            DoSpawnEnemiesToogle.onValueChanged.AddListener(value => 
            {
                EnableEnemySpawn(value);
            });

            // Enemy Spawn (Custom)
            SpawnEnemyButton.onClick.AddListener(() => SpawnEnemy());
            EnemyEditButton.onClick.AddListener(() =>
            {
                enemyEditPanelIsOpned = !enemyEditPanelIsOpned;

                EnemyEditPanel.SetActive(enemyEditPanelIsOpned);
            });

            StartWaveButton.onClick.AddListener(() => 
            {
                EnemySpawner.StartSpawning();
            });
            KillAllEnemiesButton.onClick.AddListener(() =>
            {
                EnemySpawner.ClearEnemies();
            });
        }

        private void SpawnEnemy()
        {
            if(CurrentEnemySpawnData.EnemyData == null)
            {
                Debug.Log($"Cheats. Enemy can't spawn! Enemy data is null..");
                return;
            }

            enemyEditPanelIsOpned = false;
            EnemyEditPanel.SetActive(false);

            EnemyAI enemy = Object.Instantiate(CurrentEnemySpawnData.EnemyData.EnemyPrefab, player.transform.position, Quaternion.identity).GetComponent<EnemyAI>();
            enemy.Initialize(CurrentEnemySpawnData.EnemyHp, CurrentEnemySpawnData.EnemyProtection, CurrentEnemySpawnData.EnemyData.Damage, CurrentEnemySpawnData.IsAttacking);

            EnemySpawner.AllEnemies.Add(enemy);

            LogUtility.WriteLog($"Cheats. Enemy have been spawned. Parameters: " +
                $"EnemyData='{CurrentEnemySpawnData.EnemyData.EnemyPrefab.name}', " +
                $"Health='{CurrentEnemySpawnData.EnemyHp}', " +
                $"Protection='{CurrentEnemySpawnData.EnemyProtection}'");
        }

        private void EnableEnemySpawn(bool enabled)
        {
            EnemySpawner.EnemyMaxAmount = enabled ? defaultEnemyMaxAmount : 0;

            LogUtility.WriteLog($"Cheats. Natural enemy spawn = {enabled}");
        }

        [System.Serializable]
        public class EnemySpawnData
        {
            [SerializeField] private Button ChooseEnemyDataButton;
            [SerializeField] private Image ChoosedEnemyDataVisual;
            [Space]
            [SerializeField] private TMP_InputField EnemyHealthInputField;
            [SerializeField] private TMP_InputField EnemyProtectionInputField;
            [Space]
            [SerializeField] private Toggle IsAttackingToggle;

            [Header("Choose enemy data panel")]
            [SerializeField] private GameObject EnemyDataSelectionPanel;
            [SerializeField] private Transform EnemyDataVisualsParent;
          
            [Space]
            [ReadOnly, AllowNesting] public EnemyData EnemyData;
            [ReadOnly, AllowNesting] public float EnemyHp;
            [ReadOnly, AllowNesting] public float EnemyProtection;
            [ReadOnly, AllowNesting] public bool IsAttacking;

            private bool enemyDataChoosePanelIsOpened;

            public void Initialize(EnemyData enemyData, bool isAtacking, EnemyData[] enemyList)
            {
                enemyDataChoosePanelIsOpened = false;
                EnemyDataSelectionPanel.SetActive(false);

                SetData(enemyData);

                this.IsAttacking = isAtacking;

                ChooseEnemyDataButton.onClick.AddListener(() =>
                {
                    enemyDataChoosePanelIsOpened = !enemyDataChoosePanelIsOpened;

                    EnemyDataSelectionPanel.SetActive(enemyDataChoosePanelIsOpened);
                });
                ChoosedEnemyDataVisual.sprite = enemyData.EnemyIcon;

                EnemyHealthInputField.onValueChanged.AddListener(value =>
                {
                    if(float.TryParse(value, out float health))
                    {
                        EnemyHp = health;
                    }
                    else
                    {
                        Debug.Log("Cheats. Enemy health field must not have a letters.");
                    }
                });
                EnemyProtectionInputField.onValueChanged.AddListener(value =>
                {
                    if (float.TryParse(value, out float health))
                    {
                        EnemyProtection = health;
                    }
                    else
                    {
                        Debug.Log("Cheats. Enemy health field must not have a letters.");
                    }
                });

                IsAttackingToggle.isOn = IsAttacking;
                IsAttackingToggle.onValueChanged.AddListener(value =>
                {
                    IsAttacking = value;
                });

                for (int i = 0; i < enemyList.Length; i++)
                {
                    EnemyData data = enemyList[i];

                    CheatPanel.Utilities.CreateButton_Icon($"Enemy: {data.name} ({i})", new Vector2(100f, 100f), data.EnemyIcon, EnemyDataVisualsParent, () =>
                    {
                        SetData(data);

                        ChoosedEnemyDataVisual.sprite = data.EnemyIcon;

                        enemyDataChoosePanelIsOpened = false;
                        EnemyDataSelectionPanel.SetActive(false);
                    });
                }
            }

            public void SetData(EnemyData data)
            {
                EnemyData = data;

                EnemySpawner enemySpawner = Singleton.Get<EnemySpawner>();
                EnemyHp = EnemyData.Health * enemySpawner.DifficultFactor;
                EnemyProtection = EnemyData.Protection * enemySpawner.DifficultFactor;

                EnemyHealthInputField.text = EnemyHp.ToString();
                EnemyProtectionInputField.text = EnemyProtection.ToString();
            }
        }
    }
}
