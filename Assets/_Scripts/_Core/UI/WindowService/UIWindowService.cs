using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.UI
{
    using Input;

    public class UIWindowService : IService, IEntryComponent
    {
        private const string CONFIG_PATH = "Configs/Windows Config";
        private const string ROOT_CANVAS_PATH = "Prefabs/UI/Root Canvas";

        public Canvas RootCanvas => _rootCanvas;
        public Action<WindowID> OnWindowOpened;
        public Action<WindowID> OnWindowClosed;

        private WindowsConfig _config;
        private Canvas _rootCanvas;
        private Canvas _hudCanvas;

        private Dictionary<WindowID, Window> _windows;

        public UIWindowService()
        {
            _windows = new Dictionary<WindowID, Window>();
        }

        public void Initialize()
        {
            InitConfig();
            InitRootCanvas();
        }

        private void InitConfig()
        {
            _config = Resources.Load<WindowsConfig>(CONFIG_PATH);
        }
        private void InitRootCanvas()
        {
            _rootCanvas = Object.Instantiate(Resources.Load<GameObject>(ROOT_CANVAS_PATH)).GetComponent<Canvas>();
        }

        public void SetHud(Canvas hud)
        {
            _hudCanvas = hud;
        }

        public void RegisterWindow(WindowID windowID)
        {
            RegisterWindow<Window>(windowID);
        }
        public TWindow RegisterWindow<TWindow>(WindowID windowID) where TWindow : Window
        {
            if (_windows.ContainsKey(windowID))
                return _windows[windowID] as TWindow;

            var window = Object.Instantiate(_config.GetWindow(windowID), _rootCanvas.transform);
            window.Initialize(this);
            window.OnOpened += _ =>
            {
                if (IsAnyWindowOpened())
                {
                    InputManager.Instance.SetActionMap(ActionMap.UI);
                    if (_hudCanvas) _hudCanvas.gameObject.SetActive(false);
                }
            };
            window.OnClosed += _ =>
            {
                if (!IsAnyWindowOpened())
                {
                    InputManager.Instance.SetDefaultActionMap();
                    if (_hudCanvas) _hudCanvas.gameObject.SetActive(true);
                }
            };

            _windows.Add(windowID, window);

            return window as TWindow;
        }
        public TWindow RegisterWindow<TWindow>(WindowID windowID, Window parentWindow) where TWindow : Window
        {
            if (_windows.ContainsKey(windowID))
                return _windows[windowID] as TWindow;

            var window = Object.Instantiate(_config.GetWindow(windowID), parentWindow.transform);
            window.Initialize(this);
            window.SetParentWindow(parentWindow);

            return window as TWindow;
        }
        public TWindow GetWindow<TWindow>(WindowID id) where TWindow : Window
        {
            if(_windows.ContainsKey(id))
                return _windows[id] as TWindow;

            return null;
        }

        public bool TryOpenWindow(WindowID id)
        {
            if (!_windows.ContainsKey(id))
            {
                Debug.LogWarning($"Windod by id: {id} is not registered!");
                return false;
            }

            if(!IsAnyWindowOpened())
            {
                Open(id);
                return true;
            }
            return false;
        }
        public void Open(WindowID id)
        {
            if (!_windows.ContainsKey(id))
                throw new ArgumentOutOfRangeException($"Window by id: {id} is not registered!");

            if (!_windows[id].IsOpened)
            {
                CloseAll();

                _windows[id].Open();
                OnWindowOpened?.Invoke(id);
            }
        }
        public void Close(WindowID id)
        {
            if (!_windows.ContainsKey(id))
                throw new ArgumentOutOfRangeException($"Window by id: {id} is not registered!");

            if (_windows[id].IsOpened)
            {
                _windows[id].Close();
                OnWindowClosed?.Invoke(id);
            }
        }
        public void CloseAll()
        {
            foreach (var id in _windows.Keys)
            {
                CloseWithoutCheck(id);
            }
            InputManager.Instance.SetDefaultActionMap();

            void CloseWithoutCheck(WindowID id)
            {
                if (_windows[id].IsOpened)
                {
                    _windows[id].Close();
                    OnWindowClosed?.Invoke(id);
                }
            }
        }

        public bool IsAnyWindowOpened()
        {
            foreach (var window in _windows)
            {
                if (window.Value.IsOpened)
                    return true;
            }
            return false;
        }
    }
}
