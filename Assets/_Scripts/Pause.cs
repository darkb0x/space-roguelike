using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class Pause : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        bool pauseEnabled = false;

        private void Start()
        {
            GameInput.InputActions.Player.Pause.performed += OpenClose;
            GameInput.InputActions.UI.CloseWindow.performed += OpenClose;
        }

        private void OpenClose(InputAction.CallbackContext context)
        {
            if(UIPanelManager.manager.SomethinkIsOpened())
            {
                if(UIPanelManager.manager.currentOpenedPanel != pausePanel)
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
            UIPanelManager.manager.OpenPanel(pausePanel);

            pauseEnabled = true;

            Time.timeScale = 0;
        }

        public void resume()
        {
            UIPanelManager.manager.ClosePanel(pausePanel);

            pauseEnabled = false;

            Time.timeScale = 1;
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

                UIPanelManager.manager.CloseAllPanel();
                pause();
            }
        }
    }
}
