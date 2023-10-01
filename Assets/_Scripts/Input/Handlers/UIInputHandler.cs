using UnityEngine.InputSystem;

namespace Game.Input
{
    using ActionsMap;

    public class UIInputHandler : InputHandler
    {
        public event InputCallbackDelegate PauseEvent;
        public event InputCallbackDelegate CloseEvent;

        public UIInputHandler(InputActionsMap inputActions) : base(inputActions)
        {
            _inputActions.UI.Pause.performed += OnPause;
            _inputActions.UI.CloseWindow.performed += OnCloseWindow;
        }

        public override void Dispose()
        {
            _inputActions.UI.Pause.performed -= OnPause;
            _inputActions.UI.CloseWindow.performed -= OnCloseWindow;
        }

        public override void SetActive(bool active)
        {
            if (active)
                _inputActions.UI.Enable();
            else
                _inputActions.UI.Disable();
        }

        private void OnCloseWindow(InputAction.CallbackContext context)
        {
            CloseEvent?.Invoke();
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            PauseEvent?.Invoke();
        }
    }
}
