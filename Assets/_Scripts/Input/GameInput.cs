using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class GameInput : MonoBehaviour
    {
        public static GameInput Instance;

        public static InputActions InputActions { get; private set; }

        private Camera cam;

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
            return InputActions.Player.Zoom.ReadValue<Vector2>().y;
        }
        public Vector2 GetMousePosition()
        {
            return InputActions.Player.MousePosition.ReadValue<Vector2>();
        }

        #region UI action map

        #endregion

        public void SetPlayerActionMap()
        {
            InputActions.Player.Enable();
            InputActions.UI.Disable();
        }
        public void SetUIActionMap()
        {
            InputActions.Player.Disable();
            InputActions.UI.Enable();
        }
    }
}
