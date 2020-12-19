using Economy.Buildings;
using Economy.Buildings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildButtons
{
    public class BuildOilButton : MonoBehaviour
    {
        public Player player;
        public Button button;

        private void Start()
        {
            button.gameObject.SetActive(false);
            button.onClick.AddListener(BuildOilWell);
        }

        private void BuildOilWell() =>
            Building.Build(player, new OilWell("name"));
    }
}
