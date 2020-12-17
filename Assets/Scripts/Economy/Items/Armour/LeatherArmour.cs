using Economy.Items.Armour.Base;
using Economy.Items.Base;
using Economy.Items.Inventory;

namespace Economy.Items.Armour
{
    public class LeatherArmour : IArmour
    {
        public Effect Effects { get; }
        public Item.ItemType ItemType { get; }
    }
}
