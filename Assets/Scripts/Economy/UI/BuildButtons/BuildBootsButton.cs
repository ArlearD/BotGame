using Economy.Buildings;
using Economy.Buildings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildButtons
{
    public class BuildBootsButton : MonoBehaviour
    {
        public Player player;
        public Button button;

        private void Start()
        {
            button.gameObject.SetActive(false);
            button.onClick.AddListener(BuildBoots);
        }

        private void BuildBoots() =>
            Building.Build(player, new Bootmaker("name"));
    }
}
