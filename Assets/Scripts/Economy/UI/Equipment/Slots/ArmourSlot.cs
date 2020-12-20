using Economy.Items.Armour.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Equipment.Slots
{
    public class ArmourSlot : MonoBehaviour
    {
        public Button thisButton;
        public Player player;
        public UnequipButton unequipButton;

        private IArmour _armour;

        private void Start()
        {
            thisButton.onClick.AddListener(Select);
        }

        public void SetItem(IArmour armour)
        {
            _armour = armour;
            thisButton.transform.Find("Text").GetComponent<Text>().text = armour == null ? "Nothing" : armour.ItemType.ToString();
        }


        public void Equip(IArmour armour)
        {
            SetItem(armour);
            player.EquipArmour(armour);
        }

        public void Unequip()
        {
            _armour = null;
            thisButton.transform.Find("Text").GetComponent<Text>().text = "Nothing";
            player.EquipArmour(null);
        }

        private void Select()
        {
            if (_armour != null)
                unequipButton.SetItem(_armour);
        }
    }
}
