using System;
using System.Collections.Generic;
using System.Linq;
using Economy.Items.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Economy.UI.Inventory.Market
{
    public class MarketScrollView : MonoBehaviour
    {
        public GameObject template;
        [SerializeField]
        private Player currentPlayer;
        private readonly List<Offer> _offers = new List<Offer>();
        [SerializeField]
        private Scrollbar scrollbar;
        public bool isPlayerOffers = false;

        private void Start () {
            _offers.Add(new Offer(new Item(Item.ItemType.Oil, 200), 200, "vitaliy", 1));
            _offers.Add(new Offer(new Item(Item.ItemType.Ore, 500), 1000, "mohajjid", 2));
            _offers.Add(new Offer(new Item(Item.ItemType.Wood, 3000), 2000, "maxim", 3));

            template.SetActive(false);
        }

        private void CreateOffer(Offer offer)
        {
            var rectTransform = Instantiate(template, template.transform.parent).GetComponent<RectTransform>();
            rectTransform.gameObject.SetActive(true);
            var text = rectTransform.Find("Text").GetComponent<Text>();
            text.text = offer.Item.Type + " x" + offer.Item.Amount + "\nPrice: " + offer.Price;

            var buttonText = rectTransform.Find("Button").Find("Text").GetComponent<Text>();
            buttonText.text = offer.Seller == currentPlayer.name ? "Remove" : "Buy";

            var button = rectTransform.Find("Button").GetComponent<RemoveOfferButton>();
            button.Initialize(this, offer.Id);
            rectTransform.gameObject.SetActive(true);
        }

        public void Refresh()
        {
            Clear();

            if (isPlayerOffers)
            {
                foreach (var offer in _offers.Where(x => x.Seller == currentPlayer.name))
                    CreateOffer(offer);
            }
            else
            {
                foreach (var offer in _offers.Where(x => x.Seller != currentPlayer.name))
                    CreateOffer(offer);
            }
        }

        private void Clear()
        {
            foreach (Transform child in template.transform.parent)
            {
                if (child != template.transform)
                    Destroy(child.gameObject);
            }
        }

        public bool AddOffer(Item item, int price, Player player)
        {
            if (item.Type == Item.ItemType.IronArmour ||
                item.Type == Item.ItemType.IronSword ||
                item.Type == Item.ItemType.LeatherBoots)
                if (player.RemoveItem(item))
                {
                    _offers.Add(new Offer(item, price, player.name, _offers.Count == 0 ? 1 : _offers.Max(x => x.Id) + 1));
                    Refresh();

                    return true;
                }
                else{}
            else
                if (player.RemoveResources(item.ToResources()))
                {
                    _offers.Add(new Offer(item, price, player.name, _offers.Count == 0 ? 1 : _offers.Max(x => x.Id) + 1));
                    Refresh();

                    return true;
                }

            return false;
        }

        public void RemoveOffer(int id, Player player)
        {
            var offer = _offers.FirstOrDefault(x => x.Id == id);
            if (offer == default)
                throw new Exception("This offer was not found.");
            if (offer.Seller == player.name)
            {
                if (offer.Item.Type == Item.ItemType.IronArmour ||
                    offer.Item.Type == Item.ItemType.IronSword ||
                    offer.Item.Type == Item.ItemType.LeatherBoots)
                    player.AddItems(offer.Item);
                else
                    player.AddResources(offer.Item.ToResources());
                _offers.Remove(offer);
                Refresh();
            }
            else if (player.RemoveResources(new Resources(offer.Price)))
            {
                if (offer.Item.Type == Item.ItemType.IronArmour ||
                    offer.Item.Type == Item.ItemType.IronSword ||
                    offer.Item.Type == Item.ItemType.LeatherBoots)
                    player.AddItems(offer.Item);
                else
                    player.AddResources(offer.Item.ToResources());
                _offers.Remove(offer);
                Refresh();
            }
        }

        private class Offer
        {
            public Item Item;
            public int Price;
            public string Seller;
            public int Id;

            public Offer(Item item, int price, string seller, int id)
            {
                Item = item;
                Price = price;
                Seller = seller;
                Id = id;
            }
        }
    }
}
