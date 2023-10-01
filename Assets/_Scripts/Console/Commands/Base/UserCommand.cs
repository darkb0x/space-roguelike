using System;

namespace Game.Console.Commands
{
    public abstract class UserCommand : CommandBase
    {
        protected UserCommand(string id, string help, string description) : base(id, help, description)
        {
        }

        public abstract string Invoke();
    }
    public abstract class UserCommand<T1> : CommandBase
    {
        protected UserCommand(string id, string help, string description) : base(id, help, description)
        {
        }

        public abstract string Invoke(T1 argument1);
    }
    public abstract class UserCommand<T1, T2> : CommandBase
    {
        protected UserCommand(string id, string help, string description) : base(id, help, description)
        {
        }

        public abstract string Invoke(T1 argument1, T2 argument2);
    }
}