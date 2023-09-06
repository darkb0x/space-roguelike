using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.Inventory
{
    public class MoneyAnimatedFieldVisual : MoneyFieldVisual
    {
        private readonly int _animMoneyChangedTrigger = Animator.StringToHash("moneyChanged");

        [SerializeField] private Animator Anim;
        [SerializeField] private TextMeshProUGUI DifferenceText;

        private int _currentMoney;

        protected override void UpdateField(int value)
        {
            int difference = value - _currentMoney;
            _currentMoney = value;

            if(difference < 0)
                DifferenceText.color = Color.red;
            else
                DifferenceText.color = Color.green;

            DifferenceText.text = difference.ToString();
            Anim.SetTrigger(_animMoneyChangedTrigger);

            base.UpdateField(value);
        }
    }
}