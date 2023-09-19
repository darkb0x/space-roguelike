using UnityEngine;

namespace Game.UI.HUD
{
    public abstract class HUDElement : MonoBehaviour, IHUDElement
    {
        [SerializeField] protected Canvas View;

        public abstract HUDElementID ID { get; }

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


        public void Enable()
        {
            gameObject.SetActive(true);
        }
        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}