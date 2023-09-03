namespace Play.Inventory.Service.Clients;
using System.Net.Http;
using Play.Inventory.Service.Dtos;

public class CatalogClient
{
    private readonly HttpClient httpClient;
    public CatalogClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
    {
        var items = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/item");

        return items;
    }

}