using System.Collections;
using UnityEngine;

namespace Game.UI.HUD
{
    public abstract class HUDElement : MonoBehaviour, IHUDElement, ICoroutineRunner
    {
        [SerializeField] protected Canvas View;

        public abstract HUDElementID ID { get; }

        protected HUDService _hudService;
        protected UIWindowService _uiWindowService;
        protected HUDContainer _globalHudContainer;
        protected HUDContainer _parentHudContainer;
        protected bool _initialized;

        private ICoroutineRunner _coroutineRunner;

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


        public void Enable(HUDService service, UIWindowService windowService, HUDContainer container, ICoroutineRunner coroutineRunner)
        {
            _hudService = service;
            _uiWindowService = windowService;
            _globalHudContainer = _hudService.HUDContainer;
            _parentHudContainer = container;
            _coroutineRunner = coroutineRunner;

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

        #region ICoroutineRunner
        public Coroutine RunCoroutine(IEnumerator coroutine)
        {
            return _coroutineRunner.RunCoroutine(coroutine);
        }

        public void CancelCoroutine(Coroutine coroutine)
        {
            _coroutineRunner.CancelCoroutine(coroutine);
        }
        public void CancelAllCoroutines()
        {
            _coroutineRunner.CancelAllCoroutines();
        }
        #endregion
    }
}