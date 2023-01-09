using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    public class Pause : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        bool pauseEnabled = false;

        private void Update()
        {
            if(UIPanelManager.manager.SomethinkIsOpened())
            {
                if(UIPanelManager.manager.currentOpenedPanel != pausePanel)
                {
                    return;
                }
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                pauseEnabled = !pauseEnabled;

                if(pauseEnabled)
                {
                    pause();
                }
                else
                {
                    resume();
                }
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
                UIPanelManager.manager.CloseAllPanel();
                pause();
            }
        }
    }
}
