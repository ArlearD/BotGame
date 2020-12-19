using System.Collections.Generic;
using Economy.UI.BuildingList;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildButtons
{
    public class ToggleBuildButton : MonoBehaviour
    {
        public Button thisButton;
        public ToggleListButton otherButton;
        public List<Button> buildButtons;

        private void Start()
        {
            thisButton.onClick.AddListener(Toggle);
        }

        private void Toggle()
        {
            if (otherButton.gameObject.activeSelf)
                otherButton.Deactivate();
            
            if (buildButtons[0].IsActive())
                foreach (var button in buildButtons)
                    button.gameObject.SetActive(false);
            else
                foreach (var button in buildButtons)
                    button.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            foreach (var button in buildButtons)
                button.gameObject.SetActive(false);
        }
    }
}
