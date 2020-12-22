using Economy.Items.Inventory;
using UnityEngine;

namespace Economy.Items.Base
{
    public interface IEquipment
    {
        Effect Effects { get; }

        Item.ItemType ItemType { get; }

        Sprite Icon { get; }
    }
}
