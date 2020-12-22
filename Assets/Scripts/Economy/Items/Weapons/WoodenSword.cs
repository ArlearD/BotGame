using Economy.Items.Base;
using Economy.Items.Inventory;
using Economy.Items.Weapons.Base;
using UnityEngine;

namespace Economy.Items.Weapons
{
    public class WoodenSword : IWeapon
    {
        public Effect Effects { get; }
        public Item.ItemType ItemType { get; }
        public Sprite Icon { get; }
    }
}
