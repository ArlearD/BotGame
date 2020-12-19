using System;
using Economy.Items.Inventory;

namespace Economy.Buildings.Base
{
    [Serializable]
    public abstract class ItemBuilding : Building
    {
        public abstract Item.ItemType CreatedItem { get; }

        public void CreateItem(Player player, double coeff, out int payment)
        {
            payment = 0;
            if (coeff < 1)
            {
                return;
            }

            if (player.RemoveResources(input))
            {
                player.AddItems(new Item(CreatedItem, 1));
                payment = 100;
            }
        }
    }
}
