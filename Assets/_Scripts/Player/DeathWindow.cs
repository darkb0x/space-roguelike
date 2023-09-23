using UnityEngine;
using UnityEngine.UI;

namespace Game.Player
{
    using UI;
    using SceneLoading;
    using Save;

    public class DeathWindow : Window
    {
        [SerializeField, NaughtyAttributes.Scene] private int LobbySceneID;
        [Space]
        [SerializeField] private Button RestartButton;

        public override WindowID ID => WindowID.DeathScreen;

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();

            RestartButton.onClick.AddListener(() =>
            {
                SaveManager.SessionSaveData.Reset();
                LoadSceneUtility.LoadSceneAsyncVisualize(LobbySceneID);
            });
        }
    }
}
