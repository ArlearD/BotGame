using Economy.Items.Inventory;

namespace Economy.Items.Base
{
    public interface IEquipment
    {
        Effect Effects { get; }

        Item.ItemType ItemType { get; }
    }
}
