using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Inventory.Market
{
    public class YourOffersButton : MonoBehaviour
    {
        public Button button;
        public MarketScrollView market;

        private void Start()
        {
            button.onClick.AddListener(Toggle);
        }

        private void Toggle()
        {
            market.isPlayerOffers = true;
            market.Refresh();
        }
    }
}
