using Microsoft.AspNetCore.Mvc;
using Shopping.Entities;
using Shopping.Requests;
using System.Security.Claims;

namespace Shopping.Controllers
{
    // Service-to-service endpoints; the gateway blocks /api/{service}/internal/** from the outside.
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InternalController(ShoppingDbContext db) : ControllerBase
    {
        [HttpPost("shopping-items")]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateShoppingItemRequest request, CancellationToken ct)
        {
            var newItem = new ShoppingItem()
            {
                Id = new Guid(),
                CreatedAt = DateTime.Now,
                ProductName = request.ProductName,
                Status = ShoppingItemStatus.Open,
                Quantity = request.Quantity,
                HouseholdId = request.HouseholdId,
                ProductId = request.ProductId,
                Unit = request.Unit
            };

            var result = await db.ShoppingItems.AddAsync(newItem, ct);

            await db.SaveChangesAsync(ct);

            return Ok(result.Entity.Id);
        }
    }
}
