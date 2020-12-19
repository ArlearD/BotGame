using System;
using Economy.Buildings.Base;

namespace Economy.Buildings
{
    [Serializable]
    public class OilWell : Building
    {
        public OilWell(string name)
        {
            isOn = true;
            base.name = name;
            cost = new Resources(100);
            input = new Resources();
            output = new Resources(0, 0, 100);
        }

        public override string Type => "Oil Well";
    }
}
