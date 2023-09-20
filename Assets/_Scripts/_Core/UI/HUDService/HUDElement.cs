using UnityEngine;

namespace Game.UI.HUD
{
    public abstract class HUDElement : MonoBehaviour, IHUDElement
    {
        [SerializeField] protected Canvas View;

        public abstract HUDElementID ID { get; }

        protected HUDService _hudService;
        protected HUDContainer _globalHudContainer;
        protected HUDContainer _parentHudContainer;
        protected bool _initialized;

        public virtual void Initialize()
        {
            _initialized = true;

            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            if (_initialized)
                UnsubscribeFromEvents();
        }

        protected virtual void SubscribeToEvents() { }
        protected virtual void UnsubscribeFromEvents() { }

        public virtual void Show()
        {
            View.gameObject.SetActive(true);
        }
        public virtual void Hide()
        {
            View.gameObject.SetActive(false);
        }


        public void Enable(HUDService service, HUDContainer container)
        {
            _hudService = service;
            _globalHudContainer = _hudService.HUDContainer;
            _parentHudContainer = container;

            gameObject.SetActive(true);
        }
        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public T GetHudElement<T>(HUDElementID id) where T : HUDElement
        {
            var hudElement = _hudService.GetHudElement<T>(id, _parentHudContainer);
            if(hudElement == null)
            {
                hudElement = _hudService.GetHudElement<T>(id);
            }

            return hudElement;
        }
    }
}