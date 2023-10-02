using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Game.Console
{
    using UI;
    using Input;
    using Utilities;
    using Commands;

    public class ConsoleWindow : Window
    {
        private const string COMMAND_PROMT_VISUAL_PATH = "Prefabs/UI/Elements/Console/Command Prompt";
        private const int COMMAND_PROMT_VISUAL_OBJECT_POOL_AMOUNT = 15;

        public override WindowID ID => ConsoleController.CONSOLE_WINDOW_ID;

        [SerializeField] private TextMeshProUGUI Console;
        [SerializeField] private Console_InputField ConsoleInputField;
        [Space]
        [SerializeField] private Transform CommandPromptVisualParent;

        private ConsoleInputHandler _input => InputManager.ConsoleInputHandler;

        private ConsoleController _consoleController;
        private ObjectPool<CommandPromptVisual> _commandPromtVisualObjectPool;
        private CommandPromptVisual _commandPromtVisual;

        private List<CommandPromptVisual> _loadedCommandPromtVisuals;
        private int _selectedCommandPromtVisualIndex;

        private EventSystem _eventSystem;

        public override void Initialize(UIWindowService service)
        {
            base.Initialize(service);

            _eventSystem = EventSystem.current;

            _selectedCommandPromtVisualIndex = -1;
            _loadedCommandPromtVisuals = new List<CommandPromptVisual>();
            _commandPromtVisual = Resources.Load<CommandPromptVisual>(COMMAND_PROMT_VISUAL_PATH);

            Console.text = string.Empty;
            ConsoleInputField.SetTextWithoutNotify(string.Empty);

            _commandPromtVisualObjectPool = new ObjectPool<CommandPromptVisual>(LoadCommandPromtVisual, GetCommandPromtVisual, ReturnCommandPromtVisual, COMMAND_PROMT_VISUAL_OBJECT_POOL_AMOUNT);
        }
        public void Initialize(ConsoleController consoleController)
        {
            _consoleController = consoleController;
        }
        private void OnGUI()
        {
            if (_eventSystem.currentSelectedGameObject == null)
                return;
            if(_eventSystem.currentSelectedGameObject.TryGetComponent(out Console_InputField _))
            {
                var evt = Event.current;
                if (evt.type != EventType.KeyDown && evt.type != EventType.KeyUp) return;
                if (evt.keyCode == KeyCode.UpArrow || evt.keyCode == KeyCode.DownArrow)
                    evt.Use();
            }
        }
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();

            _input.CloseEvent += _closeAction;
            _input.ConfirmSelectedEvent += ConfirmSelectedCommandPrompt;
            _input.SelectEvent += SelectCommandPrompt;

            ConsoleInputField.onSubmit.AddListener(SendCommand);
            ConsoleInputField.onValueChanged.AddListener(UpdatePrompts);
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();

            _input.CloseEvent -= _closeAction;
            _input.CloseEvent -= ConfirmSelectedCommandPrompt;
            _input.SelectEvent -= SelectCommandPrompt;
        }

        public override void Open(bool notify = true)
        {
            base.Open(notify);

            InputManager.Instance.SetActionMap(ActionMap.Console);

            ConsoleInputField.ActivateInputField();
        }

        public override void Close(bool notify = true)
        {
            base.Close(notify);

            InputManager.Instance.SetDefaultActionMap();
        }

        public void SendCommand(string text)
        {
            Console.text += $"> {text}\n";
            ConsoleInputField.text = string.Empty;
            ConsoleInputField.ActivateInputField();

            _consoleController.SendCommand(text);
        }
        public void SendResult(string text)
        {
            Console.text += $"\n{text}\n\n";
        }

        private void UpdatePrompts(string text)
        {
            _commandPromtVisualObjectPool.ReturnAll();

            if (string.IsNullOrEmpty(text))
            {
                _selectedCommandPromtVisualIndex = -1;
                return;
            }

            var commands = _consoleController.GetCommandList();
            List<CommandBase> sortedCommands = new List<CommandBase>();
            _loadedCommandPromtVisuals.Clear();
            foreach (var cmd in commands)
            {
                if (cmd.ID.StartsWith(text))
                    sortedCommands.Add(cmd);
            }

            foreach (var cmd in sortedCommands)
            {
                var visual = _commandPromtVisualObjectPool.Get();
                visual.Initialize(cmd, this);
                visual.gameObject.SetActive(true);
                _loadedCommandPromtVisuals.Add(visual);
            }
        }

        #region Select Command Prompt
        public void SelectCommandPrompt(CommandPromptVisual commandPrompt)
        {
            _selectedCommandPromtVisualIndex = _loadedCommandPromtVisuals.IndexOf(commandPrompt);
        }
        public void DeselectCommandPrompt(CommandPromptVisual commandPrompt)
        {
            if (_selectedCommandPromtVisualIndex == _loadedCommandPromtVisuals.IndexOf(commandPrompt))
                _selectedCommandPromtVisualIndex = -1;
        }
        public void SelectCommandPrompt(int direction)
        {
            _loadedCommandPromtVisuals.ForEach(x => x.Deselect());
            ConsoleInputField.MoveTextEnd(false);

            if(_loadedCommandPromtVisuals.Count == 0)
            {
                _selectedCommandPromtVisualIndex = -1;
                return;
            }

            if (_selectedCommandPromtVisualIndex + direction < 0)
                _selectedCommandPromtVisualIndex = _loadedCommandPromtVisuals.Count - 1;
            else if (_selectedCommandPromtVisualIndex + direction == _loadedCommandPromtVisuals.Count)
                _selectedCommandPromtVisualIndex = 0;
            else
                _selectedCommandPromtVisualIndex += direction;

            _loadedCommandPromtVisuals[_selectedCommandPromtVisualIndex].Select();
            ConsoleInputField.MoveTextEnd(false);
        }

        public void ConfirmSelectedCommandPrompt()
        {
            if (_selectedCommandPromtVisualIndex == -1)
                return;

            ConsoleInputField.SetTextWithoutNotify(_loadedCommandPromtVisuals[_selectedCommandPromtVisualIndex].Command.ID);
            ConsoleInputField.ActivateInputField();
            ConsoleInputField.MoveTextEnd(false);
        }
        public void ConfirmSelectedCommandPrompt(CommandPromptVisual commandPrompt)
        {
            SelectCommandPrompt(commandPrompt);
            ConfirmSelectedCommandPrompt();
        }
        #endregion

        #region Object pool func
        private CommandPromptVisual LoadCommandPromtVisual()
        {
            return Instantiate(_commandPromtVisual, CommandPromptVisualParent);
        }
        private void GetCommandPromtVisual(CommandPromptVisual commandPromptVisual)
        {
            return;
        }
        private void ReturnCommandPromtVisual(CommandPromptVisual commandPromptVisual)
        {
            commandPromptVisual.gameObject.SetActive(false);
            commandPromptVisual.ResetFields();
        }
        #endregion

        #region Static
        public static string GetTextInErrorStyle(string text)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>ERROR : {text}</color>";
        }
        public static string GetTextInWarningStyle(string text)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>WARNING : {text}</color>";
        }
        #endregion
    }
}
