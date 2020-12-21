using System;
using Economy.UI.Inventory.Market;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Inventory
{
    public class UiInventory : MonoBehaviour
    {
        private Items.Inventory.Inventory _inventory;
        private Transform _itemSlotContainer;
        private Transform _itemSlotTemplate;
        public MarketScrollView market;

        private void Start()
        {
            _itemSlotContainer = transform.Find("Item Slot Container");
            _itemSlotTemplate = _itemSlotContainer.Find("Item Slot Template");
        }

        public void SetInventory(Items.Inventory.Inventory inventory)
        {
            _inventory = inventory;
            inventory.OnItemListChanged += InventoryOnOnItemListChanged;
            Refresh();
        }

        private void InventoryOnOnItemListChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        public void Toggle()
        {
            if(gameObject.activeSelf)
                gameObject.SetActive(false);
            else
            {
                gameObject.SetActive(true);
                market.isPlayerOffers = false;
                market.Refresh();
            }
        }

        private void Refresh()
        {
            if (this != null)
            {
                foreach (Transform child in _itemSlotContainer)
                {
                    if (child != _itemSlotTemplate)
                        Destroy(child.gameObject);
                }

                var x = 0;
                var y = 0;
                var slotSize = 100f;
                foreach (var item in _inventory.ItemList)
                {
                    var itemSlotRectTransform = Instantiate(_itemSlotTemplate, _itemSlotContainer).GetComponent<RectTransform>();
                    itemSlotRectTransform.gameObject.SetActive(true);
                    itemSlotRectTransform.GetComponent<InventoryItem>().SetType(item.Type);
                    itemSlotRectTransform.anchoredPosition = new Vector2(x * slotSize, y * slotSize);
                    var text = itemSlotRectTransform.Find("Text").GetComponent<Text>();
                    text.text = item.Type + " \nx" + item.Amount;
                    x++;
                    if (x <= 10) continue;
                    x = 0;
                    y++;
                }
            }
        }
    }
}
