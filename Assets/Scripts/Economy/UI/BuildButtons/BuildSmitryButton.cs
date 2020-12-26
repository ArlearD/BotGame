﻿using Economy.Buildings;
using Economy.Buildings.Base;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildButtons
{
    public class BuildSmitryButton : MonoBehaviour
    {
        public Player player;
        public Button button;
        public GameObject card;

        private void Start()
        {
            button.gameObject.SetActive(false);
            button.onClick.AddListener(BuildSmithy);
        }

        private void BuildSmithy()
        {
            var text = card.GetComponentInChildren<Text>();
            var currentValue = Int32.Parse(text.text);
            text.text = (currentValue + 1).ToString();
            Building.Build(player, new Smity("name"));
        }
    }
}
