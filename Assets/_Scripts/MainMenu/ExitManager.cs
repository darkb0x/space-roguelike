using UnityEngine;

namespace Game.MainMenu.Pause.Exit
{
    using Utilities.LoadScene;
    using SaveData;

    public class ExitManager : MonoBehaviour
    {
        [SerializeField] private GameObject MainPanel;
        [SerializeField] private GameObject[] PanelsToDisable;
        [SerializeField, NaughtyAttributes.Scene] public int MenuSceneID;

        private void Start()
        {
            MainPanel.SetActive(false);
        }

        public void OpenExitPanel()
        {
            foreach (var panel in PanelsToDisable)
            {
                panel.SetActive(false);
            }
            MainPanel.SetActive(true);
        }
        public void CloseExitPanel()
        {
            foreach (var panel in PanelsToDisable)
            {
                panel.SetActive(true);
            }
            MainPanel.SetActive(false);
        }

        public void OpenMenu()
        {
            GameData.Instance.ResetSessionData();
            LoadSceneUtility.LoadSceneAsyncVisualize(MenuSceneID);
        }

        public void Exit()
        {
            GameData.Instance.ResetSessionData();
            Application.Quit();
        }
    }
}
