using Economy.Buildings;
using Economy.Buildings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildButtons
{
    public class BuildOreButton : MonoBehaviour
    {
        public Player player;
        public Button button;

        private void Start()
        {
            button.gameObject.SetActive(false);
            button.onClick.AddListener(BuildOreMine);
        }

        private void BuildOreMine() =>
            Building.Build(player, new OreMine("name"));
    }
}
