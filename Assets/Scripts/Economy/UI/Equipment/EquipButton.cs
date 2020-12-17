using System;
using Economy.Items.Armour.Base;
using Economy.Items.Base;
using Economy.Items.Boots.Base;
using Economy.Items.Weapons.Base;
using Economy.UI.Equipment.Slots;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Equipment
{
    public class EquipButton : MonoBehaviour
    {
        public Button thisButton;
        public ArmourSlot armourSlot;
        public WeaponSlot weaponSlot;
        public BootsSlot bootsSlot;
        public UnequipButton unequipButton;

        private IEquipment _chosenEq;

        private void Start()
        {
            thisButton.onClick.AddListener(EquipItem);
            thisButton.gameObject.SetActive(false);
        }

        public void SetItem(IEquipment item)
        {
            thisButton.gameObject.SetActive(true);
            _chosenEq = item;
            thisButton.transform.Find("Text").GetComponent<Text>().text = "Equip " + item.ItemType;
        }

        private void EquipItem()
        {
            unequipButton.gameObject.SetActive(false);
            switch (_chosenEq)
            {
                case IBoots boots:
                    bootsSlot.Equip(boots);
                    break;
                case IArmour armour:
                    armourSlot.Equip(armour);
                    break;
                case IWeapon weapon:
                    weaponSlot.Equip(weapon);
                    break;
                default:
                    throw new Exception("Couldn't find type of equipment");
            }
        }
    }
}
