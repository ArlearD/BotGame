using Economy.Buildings;
using Economy.Buildings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildButtons
{
    public class BuildWoodButton : MonoBehaviour
    {
        public Player player;
        public Button button;

        private void Start()
        {
            button.gameObject.SetActive(false);
            button.onClick.AddListener(BuildWoodCamp);
        }

        private void BuildWoodCamp() =>
            Building.Build(player, new WoodCamp("name"));
    }
}
