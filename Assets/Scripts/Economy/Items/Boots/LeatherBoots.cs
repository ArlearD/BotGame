using Economy.Items.Base;
using Economy.Items.Boots.Base;
using Economy.Items.Inventory;
using UnityEngine;

namespace Economy.Items.Boots
{
    public class LeatherBoots : IBoots
    {
        public LeatherBoots()
        {
            Effects = new Effect(0, 0, 40);
            ItemType = Item.ItemType.LeatherBoots;
            Icon = UnityEngine.Resources.Load<Sprite>("Sprites/leather_boot");
        }

        public Effect Effects { get; }
        public Item.ItemType ItemType { get; }
        public Sprite Icon { get; }
    }
}
