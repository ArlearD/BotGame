using Economy.Items.Armour.Base;
using Economy.Items.Boots.Base;
using Economy.Items.Weapons.Base;

namespace Economy
{
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
