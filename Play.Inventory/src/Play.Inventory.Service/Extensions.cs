using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Dtos;

namespace Play.Invertory.Service
{
    public static class Extensions
    {
        public static InventroyItemDto asDto(this InventoryItem item, string name, string description)
        {
            return new InventroyItemDto(item.CatalogItemId, name, description, item.Quantity, item.AcquiredDate);
        }
    }
}