﻿using Economy.Buildings;
using Economy.Buildings.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.BuildButtons
{
    public class BuildGoodsButton : MonoBehaviour
    {
        public Player player;
        public Button button;

        private void Start()
        {
            button.gameObject.SetActive(false);
            button.onClick.AddListener(BuildGoodsFactory);
        }

        private void BuildGoodsFactory() =>
            Building.Build(player, new GoodsFactory("name"));
    }
}