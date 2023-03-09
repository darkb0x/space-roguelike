using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public delegate void OnGamePaused(bool enabled);

    public class Pause : MonoBehaviour
    {

        public static Pause Instance;

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
                pause();
            }
            else
            {
                resume();
            }
        }

        public void pause()
        {
            if(UIPanelManager.Instance.OpenPanel(pausePanel, false))
            {
                Time.timeScale = 0;
                OnGamePaused?.Invoke(true);
            }
        }

        public void resume()
        {
            UIPanelManager.Instance.ClosePanel(pausePanel);
            Time.timeScale = 1;
            OnGamePaused?.Invoke(false);
        }

        public void Exit()
        {
            Application.Quit();
        }

        private void OnApplicationFocus(bool focus)
        {
            if(!focus)
            {
                if (Application.isEditor)
                    return;

                UIPanelManager.Instance.CloseAllPanel();
                pause();
            }
        }
    }
}
