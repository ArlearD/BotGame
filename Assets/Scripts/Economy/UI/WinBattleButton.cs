using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI
{
    public class WinBattleButton : MonoBehaviour
    {
        [SerializeField]
        public Button button;
        public Player player;

        private void Start()
        {
            button.onClick.AddListener(WinBattle);
        }

        private void WinBattle() =>
            player.WinBattle();
    }
}
