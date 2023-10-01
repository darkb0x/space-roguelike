using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    using ActionsMap;

    public class PlayerInputHandler : InputHandler, IScrollHandler
    {
        public event InputCallbackDelegate PauseEvent;
        public event InputCallbackDelegate BuildEvent;
        public event InputCallbackDelegate InventoryEvent;
        public event InputCallbackDelegate ConsoleEvent;

        public InputCallback InteractEvent;
        public InputCallback BreakEvent;

        public PlayerInputHandler(InputActionsMap inputActions) : base(inputActions)
        {
            _inputActions.Player.Pause.performed += OnPause;
            _inputActions.Player.Build.performed += OnBuild;
            _inputActions.Player.Inventory.performed += OnInventory;
            _inputActions.Player.Console.performed += OnConsole;

            InteractEvent = new InputCallback(_inputActions.Player.Interact);
            BreakEvent = new InputCallback(_inputActions.Player.Break);
        }

        public override void Dispose()
        {
            _inputActions.Player.Pause.performed -= OnPause;
            _inputActions.Player.Build.performed -= OnBuild;
            _inputActions.Player.Inventory.performed -= OnInventory;
            _inputActions.Player.Console.performed -= OnConsole;

            InteractEvent.Dispose();
            BreakEvent.Dispose();
        }

        public override void SetActive(bool active)
        {
            if (active)
                _inputActions.Player.Enable();
            else
                _inputActions.Player.Disable();
        }

        public Vector2 GetMoveValue()
        {
            return _inputActions.Player.Movement.ReadValue<Vector2>();
        }

        public float GetMouseScrollDeltaY()
        {
            float scrollDivision = 100f;
            return -(_inputActions.Player.Zoom.ReadValue<Vector2>().y / scrollDivision);
        }

        private void OnBuild(InputAction.CallbackContext context)
        {
            BuildEvent?.Invoke();
        }
        private void OnPause(InputAction.CallbackContext context)
        {
            PauseEvent?.Invoke();
        }
        private void OnInventory(InputAction.CallbackContext context)
        {
            InventoryEvent?.Invoke();
        }
        private void OnConsole(InputAction.CallbackContext _)
        {
            ConsoleEvent?.Invoke();
        }
    }
}
