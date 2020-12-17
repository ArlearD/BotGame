using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Inventory.Market
{
    public class RemoveOfferButton : MonoBehaviour
    {
        public Button button;
        public Player player;

        private MarketScrollView _market;
        private int _id;

        private void Start()
        {
            button.onClick.AddListener(BuyRemoveOffer);
        }

        private void BuyRemoveOffer() =>
            _market.RemoveOffer(_id, player);

        public void Initialize(MarketScrollView market, int id)
        {
            _market = market;
            _id = id;
        }
    }
}
