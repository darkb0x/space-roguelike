using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class GameInput : MonoBehaviour
    {
        public static GameInput Instance;

        public static InputActions InputActions { get; private set; }

        private Camera cam;

        private enum SelectedInputActonMap { Player, UI }
        private SelectedInputActonMap currentInputActionMap = SelectedInputActonMap.Player;

        private void Awake()
        {
            Instance = this;

            InputActions = new InputActions();
        }

        private void Start()
        {
            cam = Camera.main;

            SetPlayerActionMap();
        }

        public Vector2 GetMoveInput()
        {
            return InputActions.Player.Movement.ReadValue<Vector2>();
        }
        public float GetMouseScrollDeltaY()
        {
            float scroll = 0;
            float scrollDivision = 100f;

            switch (currentInputActionMap)
            {
                case SelectedInputActonMap.Player:
                    scroll = InputActions.Player.Zoom.ReadValue<Vector2>().y / scrollDivision;
                    break;
                case SelectedInputActonMap.UI:
                    scroll = InputActions.UI.Zoom.ReadValue<Vector2>().y / scrollDivision;
                    break;
                default:
                    goto case SelectedInputActonMap.Player;
            }
            return scroll;
        }
        public Vector2 GetMousePosition()
        {
            return Mouse.current.position.ReadValue();
        }

        #region UI action map

        #endregion

        public void SetPlayerActionMap()
        {
            currentInputActionMap = SelectedInputActonMap.Player;

            InputActions.Player.Enable();
            InputActions.UI.Disable();
        }
        public void SetUIActionMap()
        {
            currentInputActionMap = SelectedInputActonMap.UI;

            InputActions.Player.Disable();
            InputActions.UI.Enable();
        }
    }
}
