using System.Collections.Generic;
using Economy.Items.Inventory;

namespace Assets.Scripts.Economy.Data
{
    public static class MarketData
    {
        public static List<Offer> Offers { get; set; } = new List<Offer>();
    }

    public class Offer
    {
        public Item Item;
        public int Price;
        public EconomyPlayerType Seller;
        public int Id;

        public Offer(Item item, int price, EconomyPlayerType seller, int id)
        {
            Item = item;
            Price = price;
            Seller = seller;
            Id = id;
        }
    }
}
