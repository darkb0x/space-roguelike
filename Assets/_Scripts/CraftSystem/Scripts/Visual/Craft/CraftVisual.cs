using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using CraftSystem.ScriptableObjects;
using System.Linq;

namespace Game.CraftSystem.Craft.Visual
{
    using Game.CraftSystem.Visual.Category;
    using Input;

    public class CraftVisual : MonoBehaviour, IUIPanelManagerObserver
    {
        [Header("Containers")]
        [SerializeField] private List<TreeCraftContainer> Containers = new List<TreeCraftContainer>();
        [SerializeField] private CraftContainer UniqueCraftContainer;

        [Space]
        [SerializeField] private GameObject MainPanel;
        [Space]
        [SerializeField] private ScrollRect MainScrollRect;
        [Space]
        [SerializeField] private CraftCategoryController CategoryController;
        [SerializeField] private Transform ContentsParent;
        [SerializeField] private GameObject ContentPrefab;
        [Space]
        [SerializeField] private CraftNodeVisual NodeVisualPrefab;

        private UIPanelManager UIPanelManager;
        private UIInputHandler _input => InputManager.UIInputHandler;
        private CraftManager _manager;

        public void Initialize(CraftManager manager, List<CraftSO> saveData)
        {
            _manager = manager;
            UIPanelManager = ServiceLocator.GetService<UIPanelManager>();

            LoadSaveData(saveData);
            InstantiatePanels();
            InstantiateNodes();
            InitializeCategoryController();

            _input.CloseEvent += Close;
        }
        private void OnDisable()
        {
            _input.CloseEvent -= Close;
        }

        public void SelectContainer(CraftContainer container)
        {
            foreach(var item in Containers)
            {
                item.VisualParent.gameObject.SetActive(false);
            }
            UniqueCraftContainer.VisualParent.gameObject.SetActive(false);

            foreach (var item in container.GetCraftVisuals())
            {
                item.UpdateVisual();
            }
            container.VisualParent.gameObject.SetActive(true);   
            MainScrollRect.content = container.VisualParent;
        }

        private void LoadSaveData(List<CraftSO> saveData)
        {
            foreach (var craft in saveData)
            {
                if(craft is CSTreeCraftSO treeCraft)
                {
                    var container = Containers.Find(x => x.ContainerData == treeCraft.Container);

                    if(container != null)
                    {
                        container.AddCraft(craft);
                        continue;
                    }
                }

                UniqueCraftContainer.AddCraft(craft);
            }
        }
        private void InstantiatePanels()
        {
            foreach (var container in Containers)
            {
                container.InjectVisual(CreatePanel(container.Title));
            }
            UniqueCraftContainer.InjectVisual(CreatePanel(UniqueCraftContainer.Title));
            
            RectTransform CreatePanel(string name)
            {
                var panel = Instantiate(ContentPrefab, ContentsParent).GetComponent<RectTransform>();
                panel.name = $"{name} (Content)";
                return panel;
            }
        }
        private void InstantiateNodes()
        {
            foreach (var container in Containers)
            {
                foreach (var craft in container.GetCrafts())
                {
                    InstantiateNode(craft, container);
                }
            }
            foreach (var craft in UniqueCraftContainer.GetCrafts())
            {
                InstantiateNode(craft, UniqueCraftContainer);
            }

            CraftNodeVisual InstantiateNode(CraftSO craft, CraftContainer container)
            {
                CraftNodeVisual nodeVisual = Instantiate(NodeVisualPrefab, container.VisualParent);
                nodeVisual.Initialize(craft, _manager);

                container.AddCraftVisual(nodeVisual);

                return nodeVisual;
            }
        }
        private void InitializeCategoryController()
        {
            Dictionary<CraftContainer, System.Action> categories = new Dictionary<CraftContainer, System.Action>();
            foreach (var container in Containers)
            {
                categories.Add(container, () => SelectContainer(container));
            }
            categories.Add(UniqueCraftContainer, () => SelectContainer(UniqueCraftContainer));
            CategoryController.Initialize(categories);
        }

        #region Window
        public void Open()
        {
            SelectContainer(Containers.First(x => x.Enabled));

            UIPanelManager.OpenPanel(MainPanel);
        }
        public void Close()
        {
            UIPanelManager.ClosePanel(MainPanel);
        }
        public void Close(InputAction.CallbackContext callbackContext)
        {
            Close();
        }
        public void PanelStateIsChanged(GameObject panel)
        {
            if(panel != MainPanel)
            {
                MainPanel.SetActive(false);
            }
        }
        #endregion
    }
}
