using UnityEngine;
using UnityEngine.UI;

namespace Game.Menu.Pause.Exit
{
    using UI;

    public class ExitWindow : Window
    {
        public override WindowID ID => ExitManager.EXIT_WINDOW_ID;

        [SerializeField] private Button ReturnButton;
        [SerializeField] private Button MenuButton;
        [SerializeField] private Button ExitFromGameButton;

        private ExitManager _exit;

        public void Initialize(ExitManager exitManager)
        {
            _exit = exitManager;

            InitButtons();
        }

        private void InitButtons()
        {
            ReturnButton.onClick.AddListener(() => Close());
            MenuButton.onClick.AddListener(_exit.OpenMenu);
            ExitFromGameButton.onClick.AddListener(_exit.Exit);
        }
    }
}
