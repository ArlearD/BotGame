using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Inventory
{
    public class ToggleInventoryButton : MonoBehaviour
    {
        public Button thisButton;
        public UiInventory inventory;

        private void Start()
        {
            thisButton.onClick.AddListener(Toggle);
        }

        private void Toggle()
        {
            inventory.Toggle();
        }
    }
}
