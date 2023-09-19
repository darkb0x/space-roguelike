using System.Collections;
using UnityEngine;

namespace Game.CraftSystem.Oven
{
    using Input;
    using System.Collections.Generic;
    using UI;

    public class OvenVisual : Window
    {
        [SerializeField] private OvenManagerElement CraftElementPrefab;
        [SerializeField] private Transform CraftElementParent;

        public override WindowID ID => OvenManager.OVEN_WINDOW_ID;

        private UIInputHandler _input => InputManager.UIInputHandler;

        private List<OvenManagerElement> _craftVisuals;

        public void Initialize(OvenManager manager, List<OvenConfig.craft> crafts)
        {
            _craftVisuals = new List<OvenManagerElement>();

            foreach (var craft in crafts)
            {
                OvenManagerElement element = Instantiate(CraftElementPrefab, CraftElementParent);
                element.Initialize(craft, manager);

                _craftVisuals.Add(element);
            }
        }

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            _input.CloseEvent += () => _uiWindowService.Close(ID);
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            _input.CloseEvent -= () => _uiWindowService.Close(ID);
        }

        public override void Open(bool notify = true)
        {
            _craftVisuals.ForEach(x => x.UpdateData());
            base.Open(notify);
        }
    }
}