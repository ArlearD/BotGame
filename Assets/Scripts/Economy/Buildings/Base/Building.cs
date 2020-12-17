using System;
using System.Linq;

namespace Economy.Buildings.Base
{
    [Serializable]
    public abstract class Building
    {
        public string name;
        public int id;
        public abstract string Type { get; }
        public bool isOn;
        public Resources cost;
        public Resources input;
        public Resources output;

        public static void Build(Player player, Building building)
        {
            if (!player.RemoveResources(building.cost)) return;

            if (player.buildings.Count == 0)
                building.id = 0;
            else
                building.id = player.buildings.Max(b => b.id) + 1;

            player.buildings.Add(building);
            player.buildingList.AddBuilding(building);
        }
    }
}
