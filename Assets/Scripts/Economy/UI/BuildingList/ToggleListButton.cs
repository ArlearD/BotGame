using Economy.UI.BuildButtons;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildingList
{
    public class ToggleListButton : MonoBehaviour
    {
        public Button thisButton;
        public ToggleBuildButton otherButton;
        public BuildingScrollView buildingScrollView;

        private void Start()
        {
            thisButton.onClick.AddListener(Toggle);
        }

        private void Toggle()
        {
            if (otherButton.buildButtons[0].IsActive())
                otherButton.Deactivate();
            buildingScrollView.Toggle();
        }

        public void Deactivate()
        {
            if (buildingScrollView.gameObject.activeSelf)
                buildingScrollView.Toggle();
        }
    }
}
