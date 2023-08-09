using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using CraftSystem.ScriptableObjects;
using NaughtyAttributes;

namespace Game.CraftSystem.Visual.Node
{
    using Components;
    using System.Linq;
    using Utilities;

    public class CraftTreeNodeVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
#if UNITY_EDITOR
        [Foldout("Parameters"), SerializeField] private VisualNodeState m_state;        
        [Foldout("Parameters"), SerializeField] private int m_currentLevel;
        [Foldout("Parameters"), SerializeField] private CSTreeCraftSO m_currentCraft;
        [Foldout("Parameters"), SerializeField] private CSTreeCraftSO[] m_allCraftsInGroup;
        #endif

        [Header("Title")]
        [SerializeField] private TMP_Text CraftNameText;
        [Space]
        [SerializeField] private Image CraftIconImage;

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
        [SerializeField] private Image UpgradePreviewNextImage;

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
                callback => callback.gameObject.SetActive(true), 
                callback => callback.gameObject.SetActive(false),
                3);
            _visualFields = new List<NodeFieldVisual>()
                { ResearchButton, UpgradeButton, FullyUpgradedField, NonAvailableField };

            InitializeFields();

            if (researchTreeCraft.IsStartCraft())
                SetState(VisualNodeState.Purchased);
            else
                SetState(VisualNodeState.NonAvaiable);
        }
        private void InitializeFields()
        {
            ResearchButton.Initialize($"Research ({currentCraft.CraftCost})", Research);
            UpgradeButton.Initialize($"Upgrade ({_researchTreeCraft.GetNextCraft().CraftCost})", Upgrade);
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
        }
        #endif
        public void InjectSubsequentNodes(List<CraftTreeNodeVisual> subsequents)
        {
            _subsequentNodes = subsequents;
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
                    }
                    break;
                case VisualNodeState.NonPurchased:
                    {
                        ResearchButton.SetActive(true);
                        _currentNodeField = ResearchButton;
                    }
                    break;
                case VisualNodeState.Purchased:
                    {
                        UpgradeButton.SetActive(true);
                        _currentNodeField = UpgradeButton;

                        if (_subsequentNodes == null)
                            break;

                        foreach (var subsequent in _subsequentNodes)
                        {
                            subsequent.SetState(VisualNodeState.NonPurchased);
                        }
                    }
                    break;
                case VisualNodeState.FullyUpgraded:
                    {
                        FullyUpgradedField.SetActive(true);
                        _currentNodeField = FullyUpgradedField;
                    }
                    break;
                default:
                    break;
            }
            
            _pressProgress = 0f;
            UpdateFullVisual();

#if UNITY_EDITOR
            UpdateEditorDebugFields();
#endif
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

        private void UpdateFullVisual()
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
            CSTreeCraftSO next = _researchTreeCraft.GetNextCraft();

            if(next == null)
            {
                UpgradePreviewGameObj.SetActive(false);
                return;
            }

            UpgradeButton.UpdateTitleText($"Upgrade ({_researchTreeCraft.GetNextCraft().CraftCost})");

            UpgradePreviewCurrentImage.sprite = currentCraft.CraftIcon;
            UpgradePreviewNextImage.sprite = next.CraftIcon;
        }
        private void UpdateItemList()
        {
            var craft = currentCraft;

            _itemListVisualPool.ReturnAll();

            foreach (var item in craft.ItemsInCraft)
            {
                _itemListVisualPool.Get().UpdateVisual(item.Item.LowSizeIcon, item.Amount);
            }
        }

        #region Pointer Interfaces
        public void OnPointerEnter(PointerEventData eventData)
        {
            _cursorOnCraft = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _cursorOnCraft = false;
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
