using System;

namespace Economy.Items.Inventory
{
    [Serializable]
    public class Item
    {
        public enum ItemType
        {
            Default,
            Ore,
            Oil,
            Wood,
            Goods,
            IronArmour,
            LeatherBoots,
            IronSword
        }

        public ItemType Type;
        public int Amount;

        public Item(ItemType type, int amount)
        {
            Type = type;
            Amount = amount;
        }

        public Resources ToResources()
        {
            switch (Type)
            {
                case ItemType.Ore:
                    return new Resources(0, Amount);
                case ItemType.Oil:
                    return new Resources(0, 0, Amount);
                case ItemType.Wood:
                    return new Resources(0, 0, 0, Amount);
                case ItemType.Goods:
                    return  new Resources(0,0,0,0,Amount);
                default:
                    throw new NotImplementedException("This type of item is not yet supported.");
            }
        }
    }
}
