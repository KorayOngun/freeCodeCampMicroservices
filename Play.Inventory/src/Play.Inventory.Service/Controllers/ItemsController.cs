using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Entities;
using Play.Invertory.Service;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Clients;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemsRepository;
        private readonly CatalogClient catalogClient;
        public ItemsController(IRepository<InventoryItem> itemsRepository, CatalogClient catalogClient)
        {
            this.itemsRepository = itemsRepository;
            this.catalogClient = catalogClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventroyItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }
            var catalogItems = await catalogClient.GetCatalogItemsAsync();
            var inventoryItemsEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId);

            var inventoryItemDtos = inventoryItemsEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(c => c.Id == inventoryItem.CatalogItemId);
                return inventoryItem.asDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItems)
        {
            var invertoryItem = await itemsRepository.GetAsync(item => item.UserId == grantItems.UserId && item.CatalogItemId == grantItems.CatalogItemId);
            if (invertoryItem == null)
            {
                invertoryItem = new InventoryItem
                {
                    CatalogItemId = grantItems.CatalogItemId,
                    UserId = grantItems.UserId,
                    Quantity = grantItems.Quantity,
                    AcquiredDate = DateTimeOffset.Now
                };
                await itemsRepository.CreateAsync(invertoryItem);
            }
            else
            {
                invertoryItem.Quantity += grantItems.Quantity;
                await itemsRepository.UpdateAsync(invertoryItem);
            }
            return Ok();
        }
    }
}
