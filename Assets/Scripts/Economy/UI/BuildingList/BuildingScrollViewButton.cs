using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildingList
{
    public class BuildingScrollViewButton : MonoBehaviour
    {
         public int buildingId;
         public Button button;
         public Text buttonText;
         public BuildingScrollView buildingScrollView;

         private void Start()
         {
             button.onClick.AddListener(ToggleBuilding);
         }
         public void SetName(string name, int id)
         {
             buttonText.text = name;
             buildingId = id;
         }
         public void ToggleBuilding()
         {
             buildingScrollView.ButtonClicked(buildingId, button.GetComponent<Image>());
         }
    }
}
