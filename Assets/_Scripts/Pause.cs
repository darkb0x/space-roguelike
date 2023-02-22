using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class Pause : MonoBehaviour
    {
        public static Pause Instance;

        [SerializeField] private GameObject pausePanel;
        [SerializeField, NaughtyAttributes.ReadOnly] private bool m_PauseEnabled = false;

        public bool pauseEnabled { 
            get 
            {
                return m_PauseEnabled;       
            } 
            set 
            {
                m_PauseEnabled = value;
                Time.timeScale = value ? 0 : 1;
            } 
        }

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
            if(UIPanelManager.Instance.SomethinkIsOpened())
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
            UIPanelManager.Instance.OpenPanel(pausePanel);
        }

        public void resume()
        {
            UIPanelManager.Instance.ClosePanel(pausePanel);
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
