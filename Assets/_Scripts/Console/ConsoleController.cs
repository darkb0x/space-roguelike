using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Game.Console
{
    using UI;
    using Commands;
    using Input;
    using static UnityEngine.Rendering.DebugUI;

    public class ConsoleController : MonoBehaviour, IService, IEntryComponent<UIWindowService>
    {
        public const WindowID CONSOLE_WINDOW_ID = WindowID.Console;

        private PlayerInputHandler _input => InputManager.PlayerInputHandler;
        private UIWindowService _uiWindowService;

        private ConsoleWindow _consoleWindow;
        private List<CommandBase> _commands;

        public void Initialize(UIWindowService windowService)
        {
            _uiWindowService = windowService;

            InitCommands();

            _consoleWindow = windowService.RegisterWindow<ConsoleWindow>(CONSOLE_WINDOW_ID);
            _consoleWindow.Initialize(this);

            _input.ConsoleEvent += OpenConsoleWindow;
        }
        private void OnDestroy()
        {
            _input.ConsoleEvent -= OpenConsoleWindow;
        }

        public void SendCommand(string text)
        {
            string[] prompts = text.Split();

            foreach (var command in _commands)
            {
                if (command.ID != prompts[0])
                    continue;

                if(command is UserCommand uCmd)
                {
                    _consoleWindow.SendResult(uCmd.Invoke());
                    break;
                }

                if(prompts.Length == 1)
                {
                    _consoleWindow.SendResult(ConsoleWindow.GetTextInWarningStyle("Invalid arguments"));
                    break;
                }

                #region T1
                if (command is UserCommand<int> uCmdInt)
                {
                    if (int.TryParse(prompts[1], out int value))
                    {
                        _consoleWindow.SendResult(uCmdInt.Invoke(value));
                    }
                    else
                    {
                        _consoleWindow.SendResult(ConsoleWindow.GetTextInErrorStyle($"Invalid value: {prompts[1]}"));
                    }
                    break;
                }
                if(command is UserCommand<string> uCmdStr)
                {
                    _consoleWindow.SendResult(uCmdStr.Invoke(prompts[1]));
                    break;
                }
                #endregion

                if (prompts.Length == 2)
                {
                    _consoleWindow.SendResult(ConsoleWindow.GetTextInWarningStyle("Invalid arguments"));
                    break;
                }

                #region T1, T2
                if (command is UserCommand<string, int> uCmdStrInt)
                {
                    if (int.TryParse(prompts[2], out int value))
                    {
                        _consoleWindow.SendResult(uCmdStrInt.Invoke(prompts[1], value));
                    }
                    else
                    {
                        _consoleWindow.SendResult(ConsoleWindow.GetTextInErrorStyle($"Invalid value: {prompts[2]}"));
                    }
                    break;
                }
                if (command is UserCommand<int, string> uCmdIntStr)
                {
                    if (int.TryParse(prompts[1], out int value))
                    {
                        _consoleWindow.SendResult(uCmdIntStr.Invoke(value, prompts[2]));
                    }
                    else
                    {
                        _consoleWindow.SendResult(ConsoleWindow.GetTextInErrorStyle($"Invalid value: {prompts[1]}"));
                    }
                    break;
                }
                #endregion
            }
        }

        private void InitCommands()
        {
            _commands = new List<CommandBase>()
            {
                new HelpCommand(),
                new AddMoneyCommand(),
                new AddItemCommand(),
            };
        }

        public List<CommandBase> GetCommandList()
        {
            var list = new List<CommandBase>(_commands);
            list.Sort((x, y) => string.Compare(x.ID, y.ID));
            return list;
        }

        private void OpenConsoleWindow()
        {
            _uiWindowService.Open(CONSOLE_WINDOW_ID);
        }
    }
}
