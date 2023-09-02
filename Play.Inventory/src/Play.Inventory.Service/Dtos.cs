namespace Play.Invertory.Service.Dtos
{
    public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
    public record InventroyItemDto(Guid CatalogItemId, int Quantity, DateTimeOffset AcquiredDate);

}