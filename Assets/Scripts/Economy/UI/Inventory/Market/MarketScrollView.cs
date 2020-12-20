using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Economy.Data;
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
        [SerializeField]
        private Scrollbar scrollbar;
        public bool isPlayerOffers = false;

        private void Start () {
            template.SetActive(false);
        }

        private void CreateOffer(Offer offer)
        {
            var rectTransform = Instantiate(template, template.transform.parent).GetComponent<RectTransform>();
            rectTransform.gameObject.SetActive(true);
            var text = rectTransform.Find("Text").GetComponent<Text>();
            text.text = offer.Item.Type + " x" + offer.Item.Amount + "\nPrice: " + offer.Price;

            var buttonText = rectTransform.Find("Button").Find("Text").GetComponent<Text>();
            buttonText.text = offer.Seller == PlayersData.CurrentPlayer ? "Remove" : "Buy";

            var button = rectTransform.Find("Button").GetComponent<RemoveOfferButton>();
            button.Initialize(this, offer.Id);
            rectTransform.gameObject.SetActive(true);
        }

        public void Refresh()
        {
            Clear();

            if (isPlayerOffers)
            {
                foreach (var offer in MarketData.Offers.Where(x => x.Seller == PlayersData.CurrentPlayer))
                    CreateOffer(offer);
            }
            else
            {
                foreach (var offer in MarketData.Offers.Where(x => x.Seller != PlayersData.CurrentPlayer))
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
                    MarketData.Offers.Add(new Offer(item, price, PlayersData.CurrentPlayer, MarketData.Offers.Count == 0 ? 1 : MarketData.Offers.Max(x => x.Id) + 1));
                    Refresh();

                    return true;
                }
                else{}
            else
                if (player.RemoveResources(item.ToResources()))
                {
                    MarketData.Offers.Add(new Offer(item, price, PlayersData.CurrentPlayer, MarketData.Offers.Count == 0 ? 1 : MarketData.Offers.Max(x => x.Id) + 1));
                    Refresh();

                    return true;
                }

            return false;
        }

        public void RemoveOffer(int id, Player player)
        {
            var offer = MarketData.Offers.FirstOrDefault(x => x.Id == id);
            if (offer == default)
                throw new Exception("This offer was not found.");
            if (offer.Seller == PlayersData.CurrentPlayer)
            {
                if (offer.Item.Type == Item.ItemType.IronArmour ||
                    offer.Item.Type == Item.ItemType.IronSword ||
                    offer.Item.Type == Item.ItemType.LeatherBoots)
                    player.AddItems(offer.Item);
                else
                    player.AddResources(offer.Item.ToResources());
                MarketData.Offers.Remove(offer);
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

                switch (offer.Seller)
                {
                    case EconomyPlayerType.Leonardo:
                        PlayersData.Leonardo.Resources.credits += offer.Price;
                        break;
                    case EconomyPlayerType.Raphael:
                        PlayersData.Raphael.Resources.credits += offer.Price;
                        break;
                    case EconomyPlayerType.Donatello:
                        PlayersData.Donatello.Resources.credits += offer.Price;
                        break;
                    case EconomyPlayerType.Michelangelo:
                        PlayersData.Michelangelo.Resources.credits += offer.Price;
                        break;
                }
                MarketData.Offers.Remove(offer);
                Refresh();
            }
        }
    }
}
