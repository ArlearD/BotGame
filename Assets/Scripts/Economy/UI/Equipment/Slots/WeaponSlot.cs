using Economy.Items.Weapons.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Equipment.Slots
{
    public class WeaponSlot : MonoBehaviour
    {
        public Button thisButton;
        public Player player;
        public UnequipButton unequipButton;

        private IWeapon _weapon;

        private void Start()
        {
            thisButton.onClick.AddListener(Select);
        }

        public void Equip(IWeapon weapon)
        {
            _weapon = weapon;
            thisButton.transform.Find("Text").GetComponent<Text>().text = weapon.ItemType.ToString();
            player.EquipWeapon(weapon);
        }

        public void Unequip()
        {
            _weapon = null;
            thisButton.transform.Find("Text").GetComponent<Text>().text = "Nothing";
            player.EquipWeapon(null);
        }

        private void Select()
        {
            if (_weapon != null)
                unequipButton.SetItem(_weapon);
        }
    }
}
