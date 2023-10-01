using UnityEngine;

namespace Game.Console.Commands
{
    using Game.Inventory;

    public class AddMoneyCommand : UserCommand<int>
    {
        private Inventory _inventory;

        public AddMoneyCommand() : base("add_money", "add_money <count>", "Adds a money to your inventory")
        {
            _inventory = ServiceLocator.TryGetService<Inventory>();
        }

        public override string Invoke(int count)
        {
            if(_inventory == null)
            {
                return ConsoleWindow.GetTextInWarningStyle(COMMAND_ISNT_ALLOWED_TEXT);
            }
            if(count < 0)
            {
                return ConsoleWindow.GetTextInErrorStyle(string.Format(VALUE_MUST_BE_LARGER_OR_EQUAL_ZERO, nameof(count)));
            }

            _inventory.AddMoney(count);
            return $"Added {count} money to inventory";
        }
    }
}