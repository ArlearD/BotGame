using Economy.Items.Boots.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Equipment.Slots
{
    public class BootsSlot : MonoBehaviour
    {
        public Button thisButton;
        public Player player;
        public UnequipButton unequipButton;

        private IBoots _boots;

        private void Start()
        {
            thisButton.onClick.AddListener(Select);
        }

        public void Equip(IBoots boots)
        {
            _boots = boots;
            thisButton.transform.Find("Text").GetComponent<Text>().text = boots.ItemType.ToString();
            player.EquipBoots(boots);
        }

        public void Unequip()
        {
            _boots = null;
            thisButton.transform.Find("Text").GetComponent<Text>().text = "Nothing";
            player.EquipBoots(null);
        }

        private void Select()
        {
            if (_boots != null)
                unequipButton.SetItem(_boots);
        }
    }
}
