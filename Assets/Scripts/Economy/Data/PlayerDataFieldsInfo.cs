using Economy;
using Economy.Buildings.Base;
using Economy.Items.Inventory;
using System.Collections.Generic;

namespace Assets.Scripts.Economy.Data
{
    public class PlayerDataFieldsInfo
    {
        public PlayerDataFieldsInfo(Resources resources, List<Building> buildings, 
            Inventory inventory, int workersMoney, Equipment equipment)
        {
            this.resources = resources;
            this.buildings = buildings;
            this.inventory = inventory;
            this.workersMoney = workersMoney;
            this.equipment = equipment;
        }

        public void WinBattle()
        {
            resources.credits += 500 + UnityEngine.Random.Range(-250, 250);
        }

        public void LoseBattle()
        {
            resources.credits += 100 + UnityEngine.Random.Range(-25, 25);
        }

        public Resources resources { get; set; }

        public List<Building> buildings { get; set; }

        public Inventory inventory { get; set; }

        public int workersMoney { get; set; }

        public Equipment equipment { get; set; }
    }
}
