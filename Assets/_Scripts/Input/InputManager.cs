using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    using ActionsMap;

    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        [SerializeField] private ActionMap m_ActionMap;

        public static PlayerInputHandler PlayerInputHandler { get; private set; }
        public static UIInputHandler UIInputHandler { get; private set; }

        private InputActionsMap _inputActions;
        private InputHandler _selectedInputHandler;

        public InputEmptyCallbackDelegate PauseEvent;

        private Camera _camera;

        public void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _inputActions = new InputActionsMap();

            PlayerInputHandler = new PlayerInputHandler(_inputActions);
            UIInputHandler = new UIInputHandler(_inputActions);

            SetDefaultActionMap();

            PlayerInputHandler.PauseEvent += Pause;
            UIInputHandler.PauseEvent += Pause;
        }

        #region Mouse
        // Scroll
        public float GetMouseScrollDeltaY()
        { 
            if (!(_selectedInputHandler is IScrollHandler))
                return 0f;

            IScrollHandler scrollHandler = _selectedInputHandler as IScrollHandler;
            return scrollHandler.GetMouseScrollDeltaY();
        }

        // Position
        public Vector2 GetMousePosition()
        {
            return Mouse.current.position.ReadValue();
        }
        public Vector2 GetWorldMousePosition()
        {
            if (_camera == null)
                _camera = Camera.main;

            return _camera.ScreenToWorldPoint(GetMousePosition());
        }

        // Left btn
        public bool MouseLeftButtonPressed()
        {
            return Mouse.current.leftButton.isPressed;
        }
        public bool MouseLeftButtonPressed(out Vector2 pos)
        {
            pos = GetMousePosition();
            return MouseLeftButtonPressed();
        }

        // Right btn
        public bool MouseRightButtonPressed()
        {
            return Mouse.current.rightButton.isPressed;
        }
        public bool MouseRightButtonPressed(out Vector2 pos)
        {
            pos = GetMousePosition();
            return MouseRightButtonPressed();
        }
        #endregion

        public void SetDefaultActionMap()
        {
            SetActionMap(ActionMap.Player);
        }
        public void SetActionMap(ActionMap actionMap)
        {
            switch (actionMap)
            {
                case ActionMap.Player:
                    Enable(PlayerInputHandler);
                    break;
                case ActionMap.UI:
                    Enable(UIInputHandler);
                    break;
                default:
                    goto case ActionMap.Player;
            }

            m_ActionMap = actionMap;

            void Enable(InputHandler inputHandler)
            {
                PlayerInputHandler.SetActive(false);
                UIInputHandler.SetActive(false);

                inputHandler.SetActive(true);
                _selectedInputHandler = inputHandler;
            }
        }

        private void Pause()
        {
            PauseEvent?.Invoke();
        }
    }
}
