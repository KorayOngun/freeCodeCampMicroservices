using Play.Inventory.Service.Entities;
using Play.Invertory.Service.Dtos;

namespace Play.Invertory.Service
{
    public static class Extensions
    {
        public static InventroyItemDto asDto(this InventoryItem item)
        {
            return new InventroyItemDto(item.CatalogItemId, item.Quantity, item.AcquiredDate);
        }
    }
}