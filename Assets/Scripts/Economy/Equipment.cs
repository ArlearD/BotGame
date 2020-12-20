using Economy.Items.Armour.Base;
using Economy.Items.Boots.Base;
using Economy.Items.Weapons.Base;
using System;

namespace Economy
{
    [Serializable]
    public class Equipment
    {
        public IArmour Armour;
        public IWeapon Weapon;
        public IBoots Boots;

        public Equipment()
        {
            Armour = null;
            Weapon = null;
            Boots = null;
        }
    }
}
