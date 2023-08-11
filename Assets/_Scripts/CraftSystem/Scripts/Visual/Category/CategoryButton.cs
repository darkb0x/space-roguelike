using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Game.CraftSystem.Visual.Category
{
    public class CategoryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private readonly int _animEnabledBool = Animator.StringToHash("Enabled");

        [SerializeField] private Animator Anim;
        [Space]
        [SerializeField] private Image IconImage;
        [SerializeField] private TextMeshProUGUI TitleText;

        private Action OnClick;

        public void Intialize(Sprite icon, string categoryTitle, Action onClickedAction)
        {
            IconImage.sprite = icon;
            TitleText.text = categoryTitle;
            OnClick = onClickedAction;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Anim.SetBool(_animEnabledBool, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Anim.SetBool(_animEnabledBool, false);
        }
    }
}