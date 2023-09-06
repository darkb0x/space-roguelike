namespace Game.Inventory
{
    public interface IMoneyInventory
    {
        public void AddMoney(int amount);
        public bool TakeMoney(int amount);
    }
}
