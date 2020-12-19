using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Economy.Data;
using Economy.Buildings;
using Economy.Buildings.Base;
using Economy.Items.Armour.Base;
using Economy.Items.Base;
using Economy.Items.Boots.Base;
using Economy.Items.Inventory;
using Economy.Items.Weapons.Base;
using Economy.UI.BuildingList;
using Economy.UI.Inventory;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Economy
{
    public class Player : MonoBehaviour
    {
        [SerializeField] public Resources resources;//
        [SerializeField] public List<Building> buildings = new List<Building>();//
        [SerializeField] public UiInventory uiInventory;
        [SerializeField] public Text nameText;
        public BuildingScrollView buildingList;
        private Inventory inventory;//
        public string userName;
        public int workers;
        public int workersMoney;//
        public Equipment equipment;//

        public Effect Effects => equipment.Armour.Effects + equipment.Weapon.Effects + equipment.Boots.Effects;


        public void SaveData()
        {
            var player = new PlayerDataFieldsInfo(resources, buildings, inventory, workersMoney, equipment);

            switch (PlayersData.CurrentPlayer)
            {
                case CurrentPlayerEconomy.Donatello:
                    PlayersData.Donatello = player;
                    break;
                case CurrentPlayerEconomy.Leonardo:
                    PlayersData.Leonardo = player;
                    break;
                case CurrentPlayerEconomy.Michelangelo:
                    PlayersData.Michelangelo = player;
                    break;
                case CurrentPlayerEconomy.Raphael:
                    PlayersData.Raphael = player;
                    break;
            }
        }

        public void LoadData()
        {
            switch (PlayersData.CurrentPlayer)
            {
                case CurrentPlayerEconomy.Donatello:
                    SetValues(PlayersData.Donatello);
                    break;
                case CurrentPlayerEconomy.Leonardo:
                    SetValues(PlayersData.Leonardo);
                    break;
                case CurrentPlayerEconomy.Michelangelo:
                    SetValues(PlayersData.Michelangelo);
                    break;
                case CurrentPlayerEconomy.Raphael:
                    SetValues(PlayersData.Raphael);
                    break;
            }


            void SetValues(PlayerDataFieldsInfo player)
            {
                userName = PlayersData.CurrentPlayer.ToString();
                if (player != null)
                {
                    resources = player.resources;
                    buildings = player.buildings;

                    foreach (var building in buildings)
                        buildingList.AddBuilding(building);

                    inventory = player.inventory;
                    workersMoney = player.workersMoney;
                    equipment = player.equipment;
                }
            }
        }

        private void Start()
        {
            userName = "bogdan";
            resources = new Resources();
            inventory = new Inventory();
            equipment = new Equipment();
            workers = 0;

            LoadData();

            uiInventory.SetInventory(inventory);
            uiInventory.gameObject.SetActive(false);
            StartCoroutine(UpdateResources());

            nameText.text = userName;
        }

        public void WinBattle()
        {
            resources.credits += 500 + Random.Range(-250, 250);
        }

        public void LoseBattle()
        {
            resources.credits += 100 + Random.Range(-25, 25);
        }

        public void AddResources(Resources otherResources)
        {
            resources += otherResources;
            inventory.AddResources(otherResources);
        }

        public bool RemoveResources(Resources otherResources)
        {
            if (resources < otherResources)
                return false;
            resources -= otherResources;
            inventory.RemoveResources(otherResources);
            return true;
        }

        public void AddItems(Item item)
        {
            inventory.AddItem(item);
        }

        public bool RemoveItem(Item item)
        {
            return inventory.RemoveItem(item);
        }

        public void EquipArmour(IArmour newArmour)
        {
            if (newArmour != null && !RemoveItem(new Item(newArmour.ItemType, 1))) return;
            if (equipment.Armour != null)
            {
                inventory.AddItem(new Item(equipment.Armour.ItemType, 1));
            }
            equipment.Armour = newArmour;
        }

        public void EquipBoots(IBoots newBoots)
        {
            if (!RemoveItem(new Item(newBoots.ItemType, 1))) return;
            if (equipment.Boots != null)
                inventory.AddItem(new Item(equipment.Boots.ItemType, 1));
            equipment.Boots = newBoots;
        }

        public void EquipWeapon(IWeapon newWeapon)
        {
            if (!RemoveItem(new Item(newWeapon.ItemType, 1))) return;
            if (equipment.Weapon != null)
                inventory.AddItem(new Item(equipment.Weapon.ItemType, 1));
            equipment.Weapon = newWeapon;
        }

        private IEnumerator UpdateResources()
        {
            int tempWorkers;
            while (true)
            {
                tempWorkers = 0;
                foreach (var building in buildings)
                {
                    if (building.isOn)
                    {
                        var payment = resources.credits < 100 ? resources.credits : 100;
                        var coeff = payment / (double)100;

                        switch (building)
                        {
                            case Shop shop:
                                shop.SellGoods(this, coeff, out payment);
                                RemoveResources(new Resources(payment));
                                tempWorkers += payment;
                                workersMoney += payment;
                                break;
                            case ItemBuilding itemBuilding:
                                itemBuilding.CreateItem(this, coeff, out payment);
                                RemoveResources(new Resources(payment));
                                tempWorkers += payment;
                                workersMoney += payment;
                                break;
                            default:
                                if (RemoveResources(building.input * coeff))
                                {
                                    AddResources(building.output * coeff);
                                    RemoveResources(new Resources(payment));
                                    tempWorkers += payment;
                                    workersMoney += payment;
                                }
                                break;
                        }
                    }
                }

                workers = tempWorkers;
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
