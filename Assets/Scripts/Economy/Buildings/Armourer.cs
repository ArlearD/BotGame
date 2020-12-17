using Economy.Buildings.Base;
using Economy.Items.Inventory;

namespace Economy.Buildings
{
    public class Armourer : ItemBuilding
    {
        public Armourer(string name)
        {
            isOn = true;
            base.name = name;
            cost = new Resources(0, 500, 500, 500);
            input = new Resources(0, 1000);
            output = new Resources();
        }
        public override string Type => "Armourer";
        public override Item.ItemType CreatedItem => Item.ItemType.IronArmour;
    }
}
