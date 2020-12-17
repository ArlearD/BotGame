using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.ResourceValues
{
    public class OilValue : MonoBehaviour
    {
        public Player player;
        public Text text;

        private void Update()
        {
            text.text = player.resources.oil.ToString();
        }
    }
}
