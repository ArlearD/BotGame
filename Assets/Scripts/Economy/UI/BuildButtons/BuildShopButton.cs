using Economy.Buildings;
using Economy.Buildings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildButtons
{
    public class BuildShopButton : MonoBehaviour
    {
        public Player player;
        public Button button;

        private void Start()
        {
            button.gameObject.SetActive(false);
            button.onClick.AddListener(BuildShop);
        }

        private void BuildShop() =>
            Building.Build(player, new Shop("name"));
    }
}
