using Economy.Items.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Inventory.Market
{
    public class CreateOfferButton : MonoBehaviour
    {
        public Button button;
        public MarketScrollView market;
        public Player player;
        public Text title;
        public InputField count;
        public InputField price;

        private Item.ItemType _itemType = Item.ItemType.Default;

        private void Start()
        {
            button.onClick.AddListener(CreateOffer);
        }

        public void SetType(Item.ItemType type)
        {
            _itemType = type;
            title.text = type.ToString();
        }

        private void CreateOffer()
        {
            if (count.text != "" && price.text != "" && _itemType != Item.ItemType.Default)
            {
                var item = new Item(_itemType, int.Parse(count.text));
                if (market.AddOffer(item, int.Parse(price.text), player) && market.isPlayerOffers)
                    market.Refresh();
            }
        }
    }
}
