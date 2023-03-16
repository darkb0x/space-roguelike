using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.MainMenu.Pause
{
    public delegate void OnGamePaused(bool enabled);

    public class PauseManager : MonoBehaviour
    {

        public static PauseManager Instance;

        [SerializeField, Tooltip("Canvas/Pause")] private GameObject pausePanel;

        public bool pauseEnabled = false;

        public event OnGamePaused OnGamePaused;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameInput.InputActions.Player.Pause.performed += OpenClose;
            GameInput.InputActions.UI.CloseWindow.performed += OpenClose;
        }

        private void OpenClose(InputAction.CallbackContext context)
        {
            if (UIPanelManager.Instance.SomethinkIsOpened())
            {
                if(UIPanelManager.Instance.currentOpenedPanel != pausePanel)
                {
                    return;
                }
            }

            pauseEnabled = !pauseEnabled;

            if (pauseEnabled)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        public void Pause()
        {
            if(UIPanelManager.Instance.OpenPanel(pausePanel, false))
            {
                Time.timeScale = 0;
                OnGamePaused?.Invoke(true);
            }
        }

        public void Resume()
        {
            UIPanelManager.Instance.ClosePanel(pausePanel);
            Time.timeScale = 1;
            OnGamePaused?.Invoke(false);
        }

        public void Exit()
        {
            Application.Quit();
        }

        private void OnDisable()
        {
            GameInput.InputActions.Player.Pause.performed -= OpenClose;
            GameInput.InputActions.UI.CloseWindow.performed -= OpenClose;
        }

        private void OnApplicationFocus(bool focus)
        {
            if(!focus)
            {
                if (Application.isEditor)
                    return;

                UIPanelManager.Instance.CloseAllPanel();
                Pause();
            }
        }
    }
}
