using Game.Input.ActionsMap;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Game.Input
{
    public class ConsoleInputHandler : InputHandler
    {
        public InputCallbackDelegate CloseEvent;
        public InputCallbackDelegate<int> SelectEvent;
        public InputCallbackDelegate ConfirmSelectedEvent;

        public ConsoleInputHandler(InputActionsMap inputActions) : base(inputActions)
        {
            _inputActions.Console.Close.performed += OnClose;
            _inputActions.Console.Select.performed += OnSelect;
            _inputActions.Console.ConfirmSelected.performed += OnConfirmSelected;
        }

        public override void Dispose()
        {
            _inputActions.Console.Close.performed -= OnClose;
            _inputActions.Console.Select.performed -= OnSelect;
            _inputActions.Console.ConfirmSelected.performed -= OnConfirmSelected;
        }

        public override void SetActive(bool active)
        {
            if (active)
                _inputActions.Console.Enable();
            else
                _inputActions.Console.Disable();
        }

        private void OnClose(InputAction.CallbackContext _)
        {
            CloseEvent?.Invoke();
        }
        public void OnSelect(InputAction.CallbackContext context)
        {
            SelectEvent?.Invoke(Mathf.RoundToInt(context.ReadValue<float>()));
        }
        private void OnConfirmSelected(InputAction.CallbackContext _)
        {
            ConfirmSelectedEvent?.Invoke();
        }
    }
}