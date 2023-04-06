using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.MainMenu.Pause
{
    public delegate void OnGamePaused(bool enabled);

    public class PauseManager : MonoBehaviour
    {

        public static PauseManager Instance;

        [SerializeField, Tooltip("Canvas/Pause")] private GameObject MainPanel;
        [SerializeField] private GameObject[] PauseChildPanels;

        public bool pauseEnabled = false;

        public event OnGamePaused OnGamePaused;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameInput.InputActions.Player.Pause.performed += OpenClose;
            GameInput.InputActions.UI.Pause.performed += OpenClose;
        }
        private void OnDisable()
        {
            GameInput.InputActions.Player.Pause.performed -= OpenClose;
            GameInput.InputActions.UI.Pause.performed -= OpenClose;
        }

        private void OpenClose(InputAction.CallbackContext context)
        {
            if (UIPanelManager.Instance.SomethinkIsOpened())
            {
                if(UIPanelManager.Instance.currentOpenedPanel != MainPanel)
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
            UIPanelManager.Instance.OpenPanel(MainPanel);
            Time.timeScale = 0;
            OnGamePaused?.Invoke(true);
        }

        public void Resume()
        {
            foreach (var panel in PauseChildPanels)
            {
                panel.SetActive(false);
            }
            UIPanelManager.Instance.CloseAllPanel();

            Time.timeScale = 1;
            OnGamePaused?.Invoke(false);
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
