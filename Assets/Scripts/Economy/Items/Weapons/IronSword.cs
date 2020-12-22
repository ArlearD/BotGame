using Economy.Items.Base;
using Economy.Items.Inventory;
using Economy.Items.Weapons.Base;
using UnityEngine;

namespace Economy.Items.Weapons
{
    public class IronSword : IWeapon
    {
        public IronSword()
        {
            Effects = new Effect(0, 40, 0);
            ItemType = Item.ItemType.IronSword;
            Icon = UnityEngine.Resources.Load<Sprite>("Sprites/iron_sword");
        }

        public Effect Effects { get; }
        public Item.ItemType ItemType { get; }
        public Sprite Icon { get; }
    }
}
