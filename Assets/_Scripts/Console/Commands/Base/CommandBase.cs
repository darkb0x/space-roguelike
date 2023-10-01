using System;

namespace Game.Console.Commands
{
    public abstract class CommandBase
    {
        public const string COMMAND_ISNT_ALLOWED_TEXT = "This command isn't allowed here";
        public const string VALUE_MUST_BE_LARGER_OR_EQUAL_ZERO = "{0} must be larger or equal zero";
        
        protected readonly string _id;
        protected readonly string _help;
        protected readonly string _description;

        public string ID => _id;
        public string Help => _help;
        public string Description => _description;

        protected CommandBase(string id, string help, string description)
        {
            _id = id;
            _help = help;
            _description = description;
        }
    }
}
