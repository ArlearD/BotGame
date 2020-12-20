using System;
using System.Collections.Generic;

namespace Economy.Items.Inventory
{
    [Serializable]
    public class Inventory
    {
        public event EventHandler OnItemListChanged;

        public List<Item> ItemList { get; private set; }

        public Inventory()
        {
            ItemList = new List<Item>();
        }

        public void AddItem(Item item)
        {
            var itemIndex = ItemList.FindIndex(i => i.Type == item.Type);
            if (itemIndex == -1)
                ItemList.Add(item);
            else
                ItemList[itemIndex].Amount += item.Amount;
            OnItemListChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool RemoveItem(Item item)
        {
            var itemIndex = ItemList.FindIndex(i => i.Type == item.Type);
            if (itemIndex == -1 || ItemList[itemIndex].Amount < item.Amount)
                return false;
            if (ItemList[itemIndex].Amount > item.Amount)
                ItemList[itemIndex].Amount -= item.Amount;
            else ItemList.RemoveAt(itemIndex);
            OnItemListChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public void AddResources(Resources resources)
        {
            if (resources.ore > 0)
                AddItem(new Item(Item.ItemType.Ore, resources.ore));
            if (resources.oil > 0)
                AddItem(new Item(Item.ItemType.Oil, resources.oil));
            if (resources.wood > 0)
                AddItem(new Item(Item.ItemType.Wood, resources.wood));
            if (resources.goods > 0)
                AddItem(new Item(Item.ItemType.Goods, resources.goods));
        }

        public void RemoveResources(Resources resources)
        {
            if (resources.ore > 0)
                RemoveItem(new Item(Item.ItemType.Ore, resources.ore));
            if (resources.oil > 0)
                RemoveItem(new Item(Item.ItemType.Oil, resources.oil));
            if (resources.wood > 0)
                RemoveItem(new Item(Item.ItemType.Wood, resources.wood));
            if (resources.goods > 0)
                RemoveItem(new Item(Item.ItemType.Goods, resources.goods));
        }
    }
}
