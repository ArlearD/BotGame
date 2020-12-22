using Economy.Items.Armour.Base;
using Economy.Items.Base;
using Economy.Items.Inventory;
using UnityEngine;

namespace Economy.Items.Armour
{
    public class IronArmour : IArmour
    {
        public IronArmour()
        {
            Effects = new Effect(40, 0, -20);
            ItemType = Item.ItemType.IronArmour;
            Icon = UnityEngine.Resources.Load<Sprite>("Sprites/iron_armour");
        }

        public Effect Effects { get; }
        public Item.ItemType ItemType { get; }
        public Sprite Icon { get; }
    }
}
