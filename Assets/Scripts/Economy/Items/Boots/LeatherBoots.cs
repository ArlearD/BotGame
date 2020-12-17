using Economy.Items.Base;
using Economy.Items.Boots.Base;
using Economy.Items.Inventory;

namespace Economy.Items.Boots
{
    public class LeatherBoots : IBoots
    {
        public LeatherBoots()
        {
            Effects = new Effect(0, 0, 40);
            ItemType = Item.ItemType.LeatherBoots;
        }

        public Effect Effects { get; }
        public Item.ItemType ItemType { get; }
    }
}
