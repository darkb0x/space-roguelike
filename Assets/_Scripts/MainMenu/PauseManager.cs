using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.MainMenu.Pause
{
    using Input;

    public delegate void OnGamePaused(bool enabled);

    public class PauseManager : MonoBehaviour, IService, IEntryComponent<UIPanelManager>
    {
        [SerializeField, Tooltip("Canvas/Pause")] private GameObject MainPanel;
        [SerializeField] private GameObject[] PauseChildPanels;

        public bool pauseEnabled = false;

        public event OnGamePaused OnGamePaused;

        private UIPanelManager UIPanelManager;

        public void Initialize(UIPanelManager ui)
        {
            UIPanelManager = ui;

            InputManager.Instance.PauseEvent += OpenClose;
        }
        
        private void OnDisable()
        {
            InputManager.Instance.PauseEvent -= OpenClose;
        }

        private void OpenClose()
        {
            if (UIPanelManager.SomethinkIsOpened())
            {
                if(UIPanelManager.currentOpenedPanel != MainPanel)
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
            UIPanelManager.OpenPanel(MainPanel);
            Time.timeScale = 0;
            OnGamePaused?.Invoke(true);
        }

        public void Resume()
        {
            foreach (var panel in PauseChildPanels)
            {
                panel.SetActive(false);
            }
            UIPanelManager.CloseAllPanel();

            Time.timeScale = 1;
            OnGamePaused?.Invoke(false);
        }

        private void OnApplicationFocus(bool focus)
        {
            if(!focus)
            {
                if (Application.isEditor)
                    return;

                pauseEnabled = true;
                UIPanelManager.CloseAllPanel();
                Pause();
            }
        }
    }
}
