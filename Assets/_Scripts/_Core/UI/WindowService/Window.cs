using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Game.Input;

namespace Game.UI
{
    public abstract class Window : MonoBehaviour, IWindow
    {
        [Header("Window")]
        [SerializeField] protected Canvas WindowObj;

        public abstract WindowID ID { get; }
        public bool IsOpened => _isOpened;

        public Action<Window> OnOpened;
        public Action<Window> OnClosed;

        protected UIWindowService _uiWindowService;
        protected Window _parentWindow;
        protected Dictionary<WindowID, Window> _subWindows;
        protected bool _isOpened;

        protected InputCallbackDelegate _closeAction;

        #region Window Base
        // Life-cycle
        public virtual void Initialize(UIWindowService service)
        {
            _uiWindowService = service;
            _subWindows = new Dictionary<WindowID, Window>();

            _closeAction = () => _uiWindowService.Close(ID);

            SubscribeToEvents();

            Close(false);
        }
        private void OnDestroy()
            => UnsubscribeFromEvents();

        // Subscription
        protected virtual void SubscribeToEvents()
        {
            _uiWindowService.OnWindowOpened += OnAnyWindowOpened;
        }
        protected virtual void UnsubscribeFromEvents()
        {
            _uiWindowService.OnWindowOpened -= OnAnyWindowOpened;
        }

        // Open/Close
        public virtual void Open(bool notify = true)
        {
            CloseAllSubWindows(false);
            WindowObj.gameObject.SetActive(true);

            _isOpened = true;

            if(notify)
                OnOpened?.Invoke(this);
        }
        public virtual void Close(bool notify = true)
        {
            WindowObj.gameObject.SetActive(false);

            _isOpened = false;

            if(notify)
                OnClosed?.Invoke(this);
        }

        // UIWindowService.OnWindowOpened subscriber
        protected virtual void OnAnyWindowOpened(WindowID id)
        {
            if (id != ID && _isOpened)
                Close(false);
        }
        #endregion

        #region SubWindow
        // Events
        protected virtual void OnSubWindowOpened(Window subWindow)
        {
            WindowObj.gameObject.SetActive(false);
        }
        protected virtual void OnSubWindowClosed(Window subWindow)
        {
            WindowObj.gameObject.SetActive(true);
        }

        // Sub window addition
        protected void AddSubWindow(Window subWindow)
        {
            _subWindows.Add(subWindow.ID, subWindow);
            subWindow.OnOpened += OnSubWindowOpened;
            subWindow.OnClosed += OnSubWindowClosed;
        }

        // Open/Close sub window
        public void CloseAllSubWindows(bool notify = true)
        {
            foreach (var window in _subWindows)
            {
                CloseSubWindow(window.Key, notify);
            }
        }
        public void CloseSubWindow(WindowID id, bool notify = true)
        {
            _subWindows[id].Close(notify);
        }
        public void OpenSubWindow(WindowID id, bool notify = true)
        {
            _subWindows[id].Open(notify);
        }
        #endregion

        #region Public Methods
        public void SetParentWindow(Window parent)
        {
            _parentWindow = parent;
        }
        public bool IsAnySubWindowOpened()
        {
            return _subWindows.Any(x => x.Value.IsOpened);
        }
        #endregion
    }
}
