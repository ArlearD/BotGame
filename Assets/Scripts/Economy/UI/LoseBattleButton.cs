using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI
{
    public class LoseBattleButton : MonoBehaviour
    {
        [SerializeField]
        public Button button;
        public Player player;

        private void Start()
        {
            button.onClick.AddListener(LoseBattle);
        }

        private void LoseBattle() =>
            player.LoseBattle();
    }
}
