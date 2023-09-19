using UnityEngine;

namespace Game.Menu.Pause
{
    using Input;
    using UI;

    public delegate void OnGamePaused(bool enabled);

    public class PauseManager : MonoBehaviour, IService, IEntryComponent<UIWindowService>
    {
        public const WindowID PAUSE_WINDOW_ID = WindowID.Pause;

        public bool PauseEnabled = false;

        public event OnGamePaused OnGamePaused;

        private UIWindowService _uiWindowService;
        private PauseWindow _window;

        public void Initialize(UIWindowService windowService)
        {
            _uiWindowService = windowService;

            _window = _uiWindowService.RegisterWindow<PauseWindow>(PAUSE_WINDOW_ID);
            _window.OnClosed += _ => Resume();
            _window.OnOpened += _ => Pause();

            InputManager.Instance.PauseEvent += OpenClose;
        }
        
        private void OnDestroy()
        {
            InputManager.Instance.PauseEvent -= OpenClose;
        }

        private void OpenClose()
        {
            if (PauseEnabled)
            {
                if(!_window.IsAnySubWindowOpened())
                {         
                    _uiWindowService.CloseAll(); 
                }
                else
                {
                    _window.CloseAllSubWindows();
                }
            }
            else
            {
                _uiWindowService.TryOpenWindow(PAUSE_WINDOW_ID);
            }
        }

        public void Pause()
        {
            PauseEnabled = true;
            Time.timeScale = 0;
            OnGamePaused?.Invoke(true);
        }

        public void Resume()
        {
            PauseEnabled = false;
            Time.timeScale = 1;
            OnGamePaused?.Invoke(false);
        }

        private void OnApplicationFocus(bool focus)
        {
            if(!focus)
            {
                if (Application.isEditor)
                    return;

                _uiWindowService.Open(PAUSE_WINDOW_ID);
            }
        }
    }
}
