using Economy.Items.Armour;
using Economy.Items.Boots;
using Economy.Items.Inventory;
using Economy.Items.Weapons;
using Economy.UI.Equipment;
using Economy.UI.Inventory.Market;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Inventory
{
    public class InventoryItem : MonoBehaviour
    {
        public Button button;
        public CreateOfferButton createOfferButton;
        public EquipButton equipButton;

        private Item.ItemType _type;

        private void Start()
        {
            button.onClick.AddListener(SelectItem);
        }

        public void SetType(Item.ItemType type)
        {
            _type = type;
        }

        private void SelectItem()
        {
            equipButton.gameObject.SetActive(false);
            createOfferButton.SetType(_type);
            switch (_type)
            {
                case Item.ItemType.IronArmour:
                    equipButton.SetItem(new IronArmour());
                    break;
                case Item.ItemType.IronSword:
                    equipButton.SetItem(new IronSword());
                    break;
                case Item.ItemType.LeatherBoots:
                    equipButton.SetItem(new LeatherBoots());
                    break;
            }
        }
    }
}
