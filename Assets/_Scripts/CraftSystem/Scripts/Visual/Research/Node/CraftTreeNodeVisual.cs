using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using CraftSystem.ScriptableObjects;
using NaughtyAttributes;

namespace Game.CraftSystem.Research.Visual.Node
{
    using Components;
    using System.Linq;
    using Utilities;

    public class CraftTreeNodeVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private readonly int _animSelectedBool = Animator.StringToHash("Selected");

#if UNITY_EDITOR
        [Foldout("Parameters"), SerializeField] private VisualNodeState m_state;        
        [Foldout("Parameters"), SerializeField] private int m_currentLevel;
        [Foldout("Parameters"), SerializeField] private CSTreeCraftSO m_currentCraft;
        [Foldout("Parameters"), SerializeField] private CSTreeCraftSO[] m_allCraftsInGroup;
        [Foldout("Parameters"), SerializeField] private CraftTreeNodeVisual[] m_subsequents;
#endif

        [SerializeField] private Animator anim;

        [Header("Title")]
        [SerializeField] private TMP_Text CraftNameText;
        [Space]
        [SerializeField] private Image CraftIconImage;
        [Space]
        [SerializeField] private NodeOutlineVisual OutlineVisual;

        [Header("Craft Information")]
        [SerializeField] private NodeItemFieldVisual ItemVisualPrefab;
        [SerializeField] private Transform ItemVisualsParent;

        [Header("Bottom Fields")]
        [SerializeField] private NodeFieldVisual ResearchButton;
        [SerializeField] private NodeFieldVisual UpgradeButton;
        [SerializeField] private NodeFieldVisual FullyUpgradedField;
        [SerializeField] private NodeFieldVisual NonAvailableField;

        [Header("Bottom Upgrade Preview")]
        [SerializeField] private GameObject UpgradePreviewGameObj;
        [Space]
        [SerializeField] private Image UpgradePreviewCurrentImage;
        [SerializeField] private TextMeshProUGUI UpgradePreviewCurrentLevelText;
        [SerializeField] private Image UpgradePreviewNextImage;
        [SerializeField] private TextMeshProUGUI UpgradePreviewNextLevelText;

        public IReadOnlyList<CraftTreeNodeVisual> subsequentNodes { get { return _subsequentNodes; } }
        public IReadOnlyList<CSTreeCraftSO> craftsInGroup { get { return _researchTreeCraft.crafts; } }
        public CSCraftGroupSO group { get { return _researchTreeCraft.group; } } 
        public CSTreeCraftSO currentCraft { get { return _researchTreeCraft.GetCurrentCraft(); } }
        public VisualNodeState craftState { get; private set; }
        public string craftName { get; private set; }

        private ResearchManager _manager;
        private ResearchTreeCraft _researchTreeCraft;

        private List<CraftTreeNodeVisual> _subsequentNodes;

        private List<NodeFieldVisual> _visualFields;
        private ObjectPool<NodeItemFieldVisual> _itemListVisualPool;

        private NodeFieldVisual _currentNodeField;
        private float m_pressProgress;
        private float _pressProgress
        {
            get
            {
                return m_pressProgress;
            }
            set
            {
                m_pressProgress = value;
                _currentNodeField?.UpdateFill(m_pressProgress);
                OutlineVisual.UpdateFill(m_pressProgress);
            }
        }
        private bool _cursorOnCraft;
        private bool _isPressed;

        private Coroutine _cancelPressProgressCoroutine;

        public void Initalize(string craftName, ResearchTreeCraft researchTreeCraft, ResearchManager manager)
        {
            this.craftName = craftName;
            CraftNameText.text = craftName;
            _researchTreeCraft = researchTreeCraft;

            _manager = manager;

            _itemListVisualPool = new ObjectPool<NodeItemFieldVisual>(
                () => Instantiate(ItemVisualPrefab, ItemVisualsParent),
                callback => callback.Enable(), 
                callback => callback.Disable(),
                3);
            _visualFields = new List<NodeFieldVisual>()
                { ResearchButton, UpgradeButton, FullyUpgradedField, NonAvailableField };
            _subsequentNodes = new List<CraftTreeNodeVisual>();

            InitializeFields();

            if (researchTreeCraft.IsStartCraft())
            {
                if (researchTreeCraft.crafts[0].CraftCost == 0)
                {
                    SetState(VisualNodeState.Purchased);
                    _manager.Research(researchTreeCraft);
                }
                else
                {
                    SetState(VisualNodeState.NonPurchased);
                }
            }
            else
            {
                SetState(VisualNodeState.NonAvaiable);
            }
        }
        private void InitializeFields()
        {
            ResearchButton.Initialize($"Research ({currentCraft.CraftCost}$)", Research);
            UpgradeButton.Initialize($"Upgrade ({_researchTreeCraft.GetNextCraft().CraftCost}$)", Upgrade);
            FullyUpgradedField.Initialize("Upgraded!", null);
            NonAvailableField.Initialize("Non Avaialable", null);
        }
        #if UNITY_EDITOR
        private void UpdateEditorDebugFields()
        {
            m_state = craftState;
            m_currentLevel = _researchTreeCraft.level;
            m_currentCraft = _researchTreeCraft.GetCurrentCraft();
            m_allCraftsInGroup = _researchTreeCraft.crafts.ToArray();
            m_subsequents = subsequentNodes.ToArray();
        }
        #endif
        public void InjectSubsequentNodes(List<CraftTreeNodeVisual> subsequents)
        {
            _subsequentNodes = subsequents;
            UpdateState();
        }

        public void UpdateState()
        {
            SetState(craftState);
        }
        public void SetState(VisualNodeState state)
        {
            craftState = state;

            foreach (var field in _visualFields) // disable all fields
            {
                field.SetActive(false);
            }

            switch (craftState)
            {
                case VisualNodeState.NonAvaiable:
                    {
                        NonAvailableField.SetActive(true);
                        _currentNodeField = NonAvailableField;

                        SetHideView();
                    }
                    break;
                case VisualNodeState.NonPurchased:
                    {
                        ResearchButton.SetActive(true);
                        _currentNodeField = ResearchButton;

                        SetDefaultView();
                    }
                    break;
                case VisualNodeState.Purchased:
                    {
                        UpgradeButton.SetActive(true);
                        _currentNodeField = UpgradeButton;

                        SetDefaultView();

                        if (_subsequentNodes == null)
                            break;

                        foreach (var subsequent in _subsequentNodes)
                        {
                            subsequent.UnlockForBuy();
                        }
                    }
                    break;
                case VisualNodeState.FullyUpgraded:
                    {
                        FullyUpgradedField.SetActive(true);
                        _currentNodeField = FullyUpgradedField;

                        SetDefaultView();
                    }
                    break;
                default:
                    break;
            }

            OutlineVisual.SetColor(_currentNodeField.MainImage.color, _currentNodeField.FillImage.color);
            _pressProgress = 0f;
            UpdateFullVisual();

#if UNITY_EDITOR
            UpdateEditorDebugFields();
#endif
        }
        protected void UnlockForBuy()
        {
            if(craftState == VisualNodeState.NonAvaiable)
            {
                SetState(VisualNodeState.NonPurchased);
            }
        }

        private void Update()
        {
            if(!_cursorOnCraft) return;
            if (!_isPressed) return;

            float speed = 2f;
            _pressProgress = Mathf.Clamp(_pressProgress + Time.deltaTime * speed, 0, 1);

            if (_pressProgress == 1)
            {
                _currentNodeField.Activate();
                _pressProgress = 0;
            }
        }

        private void Research()
        {
            _manager.Research(_researchTreeCraft, this);

            #if UNITY_EDITOR
            UpdateEditorDebugFields();
            #endif
        }
        private void Upgrade()
        {
            _manager.Upgrade(_researchTreeCraft, this);

            #if UNITY_EDITOR
            UpdateEditorDebugFields();
            #endif 
        }

        #region Visual Update Methods
        private void SetDefaultView()
        {
            CraftNameText.text = craftName;
            CraftIconImage.color = Color.white;
        }
        private void SetHideView()
        {
            CraftNameText.text = "???";
            CraftIconImage.color = Color.black;
        }

        public void UpdateFullVisual()
        {
            CraftIconImage.sprite = currentCraft.CraftIcon;

            UpdateUpgradePreview();
            UpdateItemList();
        }

        private IEnumerator CancelPressProgress()
        {  
            float speed = 3f;

            while(_pressProgress > 0)
            {
                yield return null;

                _pressProgress = Mathf.MoveTowards(_pressProgress, 0, speed * Time.deltaTime);
            }

            _cancelPressProgressCoroutine = null;
        }
        private void UpdateUpgradePreview()
        {
            if(craftState == VisualNodeState.NonPurchased | craftState == VisualNodeState.NonAvaiable)
            {
                UpgradePreviewGameObj.SetActive(false);
                return;
            }

            CSTreeCraftSO next = _researchTreeCraft.GetNextCraft();

            if(next == null)
            {
                UpgradePreviewGameObj.SetActive(false);
                return;
            }

            UpgradePreviewGameObj.SetActive(true);

            UpgradeButton.UpdateTitleText($"Upgrade ({_researchTreeCraft.GetNextCraft().CraftCost}$)");

            UpgradePreviewCurrentImage.sprite = currentCraft.CraftIcon;
            UpgradePreviewCurrentLevelText.text = "Lvl. " + (_researchTreeCraft.IndexOf(currentCraft) + 1);
            UpgradePreviewNextImage.sprite = next.CraftIcon;
            UpgradePreviewNextLevelText.text = "Lvl. " + (_researchTreeCraft.IndexOf(next) + 1);
        }
        private void UpdateItemList()
        {
            var craft = currentCraft;

            _itemListVisualPool.ReturnAll();

            foreach (var item in craft.ItemsInCraft)
            {
                var itemVisual = _itemListVisualPool.Get();

                if (craftState == VisualNodeState.NonAvaiable)
                    itemVisual.SetHideStyle();
                else
                    itemVisual.SetDefaultStyle();

                itemVisual.UpdateVisual(item.Item.LowSizeIcon, item.Amount);
            }
        }
        #endregion

        #region Pointer Interfaces
        public void OnPointerEnter(PointerEventData eventData)
        {
            _cursorOnCraft = true;

            anim.SetBool(_animSelectedBool, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _cursorOnCraft = false;

            anim.SetBool(_animSelectedBool, false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;

            if(_cancelPressProgressCoroutine != null)
                StopCoroutine(_cancelPressProgressCoroutine);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;

            _cancelPressProgressCoroutine = StartCoroutine(CancelPressProgress());
        }
        #endregion
    }
}
