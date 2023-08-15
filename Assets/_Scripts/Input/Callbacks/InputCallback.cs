using UnityEngine.InputSystem;

namespace Game.Input
{
    public class InputCallback
    {
        public InputEmptyCallbackDelegate Performed;
        public InputEmptyCallbackDelegate Canceled;

        private InputAction _action;

        public InputCallback(InputAction inputAction)
        {
            _action = inputAction;

            _action.performed += UpdateState;
            _action.canceled += UpdateState;
        }

        public bool IsPressed()
        {
            return _action.IsPressed();
        }

        public void Dispose()
        {
            _action.performed -= UpdateState;
            _action.canceled -= UpdateState;
        }

        private void UpdateState(InputAction.CallbackContext callback)
        {
            if(callback.performed)
            {
                Performed?.Invoke();
            }
            else if(callback.canceled)
            {
                Canceled?.Invoke();
            }
        }
    }
}
