using UnityEngine;

namespace Game.Console.Commands
{
    using Inventory;

    public class AddItemCommand : UserCommand<string, int>
    {
        private const string ITEM_PATH = "Items/";

        private IInventory _inventory;

        public AddItemCommand() : base("add_item", "add_item <item> <amount>", "Adds item to inventory")
        {
            _inventory = ServiceLocator.TryGetService<IInventory>();
        }

        public override string Invoke(string itemName, int amount)
        {
            if(_inventory == null)
            {
                return ConsoleWindow.GetTextInWarningStyle(COMMAND_ISNT_ALLOWED_TEXT);
            }
            if(amount < 0)
            {
                return ConsoleWindow.GetTextInErrorStyle(string.Format(VALUE_MUST_BE_LARGER_OR_EQUAL_ZERO, nameof(amount)));
            }
            string itemPath = itemName.Replace(":", "/");
            InventoryItem item = Resources.Load<InventoryItem>(ITEM_PATH + itemPath);
            if(item == null)
            {
                return ConsoleWindow.GetTextInErrorStyle($"Item with name: '{itemName}' is not exist");
            }

            _inventory.AddItem(new ItemData(item, amount), false);

            return $"Added {amount} {item.ItemName}";
        }
    }
}