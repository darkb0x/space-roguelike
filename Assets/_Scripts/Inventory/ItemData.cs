namespace Game.Inventory
{
    [System.Serializable]
    public class ItemData
    {
        [NaughtyAttributes.Expandable] public InventoryItem Item;
        public int Amount;

        public ItemData(InventoryItem item, int amount = 0)
        {
            Item = item;
            Amount = amount;
        }

        public override string ToString()
        {
            if(Item != null)
                return $"{Item.ItemName} ({Amount})";
            return $"NONE ({Amount})";
        }
    }
}
