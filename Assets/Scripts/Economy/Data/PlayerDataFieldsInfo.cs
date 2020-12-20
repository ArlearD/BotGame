using System.Collections.Generic;
using Economy;
using Economy.Buildings.Base;
using Economy.Items.Inventory;
using UnityEngine;
using Resources = Economy.Resources;

namespace Assets.Scripts.Economy.Data
{
    [System.Serializable]
    public class PlayerDataFieldsInfo
    {

        public Resources Resources { get; set; }

        public List<Building> Buildings { get; set; }

        public Inventory Inventory { get; set; }

        public int WorkersMoney { get; set; }

        public Equipment Equipment { get; set; }

        public PlayerDataFieldsInfo()
        {
            Resources = new Resources();
            Buildings = new List<Building>();
            Inventory = new Inventory();
            WorkersMoney = 0;
            Equipment = new Equipment();
        }

        public PlayerDataFieldsInfo(Resources resources, List<Building> buildings,
            Inventory inventory, int workersMoney, Equipment equipment)
        {
            Resources = resources;
            Buildings = buildings;
            Inventory = inventory;
            WorkersMoney = workersMoney;
            Equipment = equipment;
        }

        public void WinBattle()
        {
            Resources.credits += 500 + Random.Range(-250, 250);
        }

        public void LoseBattle()
        {
            Resources.credits += 100 + Random.Range(-25, 25);
        }
    }
}
