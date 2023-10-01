namespace Game.Console.Commands
{
    public class HelpCommand : UserCommand
    {
        private ConsoleController _consoleController;

        public HelpCommand() : base("help", "help", "Send a command list")
        {
            _consoleController = ServiceLocator.GetService<ConsoleController>();
        }

        public override string Invoke()
        {
            string result = "Commands:";
            foreach (var cmd in _consoleController.GetCommandList())
            {
                result += $"\n{cmd.Help} - {cmd.Description}";
            }
            return result;
        }
    }
}