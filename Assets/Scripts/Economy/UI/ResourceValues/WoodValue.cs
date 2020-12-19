using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.ResourceValues
{
    public class WoodValue : MonoBehaviour
    {
        public Player player;
        public Text text;

        private void Update()
        {
            text.text = player.resources.wood.ToString();
        }
    }
}
