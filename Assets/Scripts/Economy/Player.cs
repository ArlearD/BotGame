using System.Collections;
using System.Collections.Generic;
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
using Random = UnityEngine.Random;

namespace Economy
{
    public class Player : MonoBehaviour
    {
        [SerializeField] public Resources resources;
        [SerializeField] public List<Building> buildings = new List<Building>();
        [SerializeField] public UiInventory uiInventory;
        public BuildingScrollView buildingList;
        private Inventory inventory;
        public string name;
        public int workers;
        public int workersMoney;
        public Equipment equipment;

        public Effect Effects => equipment.Armour.Effects + equipment.Weapon.Effects + equipment.Boots.Effects;

        private void Start()
        {
            name = "bogdan";
            resources = new Resources();
            inventory = new Inventory();
            equipment = new Equipment();
            workers = 0;
            uiInventory.SetInventory(inventory);
            uiInventory.gameObject.SetActive(false);
            StartCoroutine(UpdateResources());
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
                        var coeff = payment / (double) 100;

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
