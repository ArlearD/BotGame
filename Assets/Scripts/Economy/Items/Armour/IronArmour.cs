using Economy.Items.Armour.Base;
using Economy.Items.Base;
using Economy.Items.Inventory;

namespace Economy.Items.Armour
{
    public class IronArmour : IArmour
    {
        public IronArmour()
        {
            Effects = new Effect(40, 0, -20);
            ItemType = Item.ItemType.IronArmour;
        }

        public Effect Effects { get; }
        public Item.ItemType ItemType { get; }
    }
}
