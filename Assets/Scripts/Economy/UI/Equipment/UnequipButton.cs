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
    public class UnequipButton : MonoBehaviour
    {
        public Button thisButton;
        public ArmourSlot armourSlot;
        public WeaponSlot weaponSlot;
        public BootsSlot bootsSlot;

        private IEquipment _chosenEq;

        private void Start()
        {
            thisButton.onClick.AddListener(UnequipItem);
            thisButton.gameObject.SetActive(false);
        }

        public void SetItem(IEquipment item)
        {
            thisButton.gameObject.SetActive(true);
            _chosenEq = item;
            thisButton.transform.Find("Text").GetComponent<Text>().text = "Unequip " + item.ItemType;
        }

        private void UnequipItem()
        {
            switch (_chosenEq)
            {
                case IBoots boots:
                    bootsSlot.Unequip();
                    break;
                case IArmour armour:
                    armourSlot.Unequip();
                    break;
                case IWeapon weapon:
                    weaponSlot.Unequip();
                    break;
                default:
                    throw new Exception("Couldn't find type of equipment");
            }

            thisButton.gameObject.SetActive(false);
        }
    }
}
