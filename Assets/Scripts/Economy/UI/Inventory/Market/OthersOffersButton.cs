using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Inventory.Market
{
    public class OthersOffersButton : MonoBehaviour
    {
        public Button button;
        public MarketScrollView market;

        private void Start()
        {
            button.onClick.AddListener(Toggle);
        }

        private void Toggle()
        {
            market.isPlayerOffers = false;
            market.Refresh();
        }
    }
}
