using Economy.Buildings.Base;

namespace Economy.Buildings
{
    public class GoodsFactory : Building
    {
        public GoodsFactory(string name)
        {
            isOn = true;
            base.name = name;
            cost = new Resources(0, 500, 500, 500);
            input = new Resources(0, 100, 100, 100);
            output = new Resources(0, 0, 0, 0, 100);
        }

        public override string Type => "Goods Factory";
    }
}
