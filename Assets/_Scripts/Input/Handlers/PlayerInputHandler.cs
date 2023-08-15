using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    using ActionsMap;

    public class PlayerInputHandler : InputHandler, IScrollHandler
    {
        public event InputEmptyCallbackDelegate PauseEvent;
        public event InputEmptyCallbackDelegate BuildEvent;
        public event InputEmptyCallbackDelegate InventoryEvent;

        public InputCallback InteractEvent;
        public InputCallback BreakEvent;

        public PlayerInputHandler(InputActionsMap inputActions) : base(inputActions)
        {
            _inputActions.Player.Pause.performed += OnPause;
            _inputActions.Player.Build.performed += OnBuild;
            _inputActions.Player.Inventory.performed += OnInventory;

            InteractEvent = new InputCallback(_inputActions.Player.Interact);
            BreakEvent = new InputCallback(_inputActions.Player.Break);
        }

        public override void Dispose()
        {
            _inputActions.Player.Pause.performed -= OnPause;
            _inputActions.Player.Build.performed -= OnBuild;
            _inputActions.Player.Inventory.performed -= OnInventory;

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

        public void OnBuild(InputAction.CallbackContext context)
        {
            BuildEvent?.Invoke();
        }


        public void OnPause(InputAction.CallbackContext context)
        {
            PauseEvent?.Invoke();
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            InventoryEvent?.Invoke();
        }
    }
}
