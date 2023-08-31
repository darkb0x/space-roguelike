using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using CraftSystem.ScriptableObjects;

namespace Game.CraftSystem.Craft.Visual
{
    using Player.Inventory;

    public class CraftNodeVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private readonly int _animSelectedBool = Animator.StringToHash("Selected");

        [SerializeField] private TextMeshProUGUI CraftNameText;
        [SerializeField] private Image CraftIconImage;
        [Space]
        [SerializeField] private Animator Anim;
        [Space]
        [SerializeField] private Image PressProgressImage;
        [Space]
        [SerializeField] private Transform ItemsVisualParent;
        [SerializeField] private NodeCraftItemField ItemVisualPrefab;

        private PlayerInventory PlayerInventory;
        private CraftManager _manager;

        private CraftSO craft;
        private NodeCraftItemField[] _itemVisualFields;

        private Coroutine _cancelPressProgressCoroutine;
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
                PressProgressImage.fillAmount = m_pressProgress;
            }
        }
        private bool _cursorOnCraft;
        private bool _isPressed;

        public void Initialize(CraftSO craft, CraftManager manager)
        {
            PlayerInventory = ServiceLocator.GetService<PlayerInventory>();
            this.craft = craft;
            _manager = manager;

            _itemVisualFields = new NodeCraftItemField[craft.ItemsInCraft.Count];
            for (int i = 0; i < craft.ItemsInCraft.Count; i++)
            {
                _itemVisualFields[i] = Instantiate(ItemVisualPrefab, ItemsVisualParent);
                _itemVisualFields[i].Initialize(craft.ItemsInCraft[i]);
            }

            if(craft is CSTreeCraftSO)
            {
                InitializeAsTreeCraft();
            }
            else
            {
                InitializeAsCraft();
            }
        }
        private void InitializeAsCraft()
        {
            CraftNameText.text = craft.CraftName;
            CraftIconImage.sprite = craft.CraftIcon;
        }
        private void InitializeAsTreeCraft()
        {
            var treeCraft = craft as CSTreeCraftSO;

            CraftNameText.text = treeCraft.Group.GroupName;
            CraftIconImage.sprite = treeCraft.CraftIcon;
        }

        private void Update()
        {
            if (!_cursorOnCraft)
                return;
            if (!_isPressed)
                return;

            float speed = 2f;
            _pressProgress = Mathf.Clamp(_pressProgress + Time.deltaTime * speed, 0, 1);

            if(_pressProgress == 1)
            {
                _manager.Craft(craft);

                if(gameObject.activeInHierarchy)
                    _cancelPressProgressCoroutine = StartCoroutine(CancelPressProgress(1.3f));
            }
        }

        public void UpdateVisual()
        {
            _pressProgress = 0;
            _cursorOnCraft = false;
            _isPressed = false;

            for (int i = 0; i < craft.ItemsInCraft.Count; i++)
            {
                ItemData curCraft = craft.ItemsInCraft[i];
                ItemData inventoryItem = PlayerInventory.GetItem(curCraft.Item);
                if (inventoryItem != null)
                {
                    if(inventoryItem.Amount >= curCraft.Amount)
                    {
                        _itemVisualFields[i].SetEnoughStyle();
                    }
                    else
                    {
                        _itemVisualFields[i].SetNotEnoughStyle();
                    }
                }
                else
                {
                    _itemVisualFields[i].SetNotEnoughStyle();
                }
            }
        }

        private IEnumerator CancelPressProgress(float speedFactor = 1)
        {
            float speed = 3f * speedFactor;

            while (_pressProgress > 0)
            {
                yield return null;

                _pressProgress = Mathf.MoveTowards(_pressProgress, 0, speed * Time.deltaTime);
            }

            _cancelPressProgressCoroutine = null;
        }

        #region Pointer Interfaces
        public void OnPointerEnter(PointerEventData eventData)
        {
            _cursorOnCraft = true;

            Anim.SetBool(_animSelectedBool, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _cursorOnCraft = false;

            Anim.SetBool(_animSelectedBool, false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;

            if (_cancelPressProgressCoroutine != null)
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