using Economy.Items.Base;
using Economy.Items.Boots.Base;
using Economy.Items.Inventory;
using UnityEngine;

namespace Economy.Items.Boots
{
    public class IronBoots : IBoots
    {
        public Effect Effects { get; }
        public Item.ItemType ItemType { get; }
        public Sprite Icon { get; }
    }
}
