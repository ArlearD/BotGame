using System;
using Economy.Buildings.Base;

namespace Economy.Buildings
{
    [Serializable]
    public class OreMine : Building
    {
        public OreMine(string name)
        {
            isOn = true;
            base.name = name;
            cost = new Resources(100);
            input = new Resources();
            output = new Resources(0, 100);
        }

        public override string Type => "Ore Mine";
    }
}
