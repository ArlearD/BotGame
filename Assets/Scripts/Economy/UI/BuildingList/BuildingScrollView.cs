using System;
using System.Collections.Generic;
using System.Linq;
using Economy.Buildings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildingList
{
    public class BuildingScrollView : MonoBehaviour
    {
        public GameObject buttonTemplate;
        [SerializeField]
        private Player player;
        private readonly List<BuildingScrollViewButton> _buttons = new List<BuildingScrollViewButton>();
        [SerializeField]
        private Scrollbar scrollbar;

        private void Start () {
            buttonTemplate.SetActive(false);

            foreach(var building in player.buildings)
            {
                var buttonGameObject = Instantiate(buttonTemplate, buttonTemplate.transform.parent, true);
                buttonGameObject.SetActive(false);
                var button = buttonGameObject.GetComponent<BuildingScrollViewButton>();
                button.SetName(CreateButtonName(building), building.id);
                if (!building.isOn)
                    button.GetComponent<Image>().color = Color.grey;
                _buttons.Add(button);
            }

            gameObject.SetActive(false);
            scrollbar.gameObject.SetActive(false);
        }

        public void AddBuilding(Building building)
        {
            var buttonGameObject = Instantiate(buttonTemplate, buttonTemplate.transform.parent, true);
            buttonGameObject.SetActive(gameObject.activeSelf);
            var button = buttonGameObject.GetComponent<BuildingScrollViewButton>();
            button.SetName(CreateButtonName(building), building.id);
            _buttons.Add(button);
        }

        //TODO проверить работает чи да чи нет
        public void RemoveBuilding(int id)
        {
            var button = _buttons.FirstOrDefault(b => b.buildingId == id);
            if (button != default)
                Destroy(button);
            _buttons.Remove(button);
        }

        private string CreateButtonName(Building building) => building.Type + " " + building.name;

        public void Toggle()
        {
            if (gameObject.activeSelf)
            {
                foreach (var button in _buttons)
                    button.gameObject.SetActive(false);
                scrollbar.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                scrollbar.gameObject.SetActive(true);
                foreach (var button in _buttons)
                    button.gameObject.SetActive(true);
            }
        }

        public void ButtonClicked(int id, Image image)
        {
            var building = player.buildings.FirstOrDefault(x => x.id == id);
            if (building == default)
                throw new Exception($"Couldn't find building with id {id}");
            if (building.isOn)
            {
                building.isOn = false;
                image.color = Color.grey;
            }
            else
            {
                building.isOn = true;
                image.color = Color.white;
            }
        }
    }
}
