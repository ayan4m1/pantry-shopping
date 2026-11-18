using Microsoft.AspNetCore.Mvc;
using Shopping.Entities;
using System.Security.Claims;

namespace Shopping.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingController(ShoppingDbContext db) : ControllerBase
    {
        [HttpPut("create")]
        [EndpointName("createShoppingItem")]
        public async Task<ActionResult<Guid>> CreateShoppingItem([FromQuery] Guid productId)
        {
            var userId = User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userId))
            {
                return NotFound();
            }

            var product = await db.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            var newItem = new ShoppingItem()
            {
                UserId = userId,
                ProductId = product.Id,
                Purchased = false
            };

            var result = await db.ShoppingItems.AddAsync(newItem);

            await db.SaveChangesAsync();

            return Ok(result.Entity.Id);
        }

        [HttpPost("set-state")]
        [EndpointName("setShoppingItemState")]
        public async Task<ActionResult<bool>> SetShoppingItemState([FromQuery] Guid productId, [FromQuery] bool purchased)
        {
            var userId = User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userId))
            {
                return NotFound();
            }

            var shoppingItem = await db.ShoppingItems.FindAsync(productId);

            if (shoppingItem == null)
            {
                return NotFound();
            }

            shoppingItem.Purchased = purchased;

            await db.SaveChangesAsync();

            return Ok(true);
        }

        [HttpDelete("delete")]
        [EndpointName("deleteShopingItem")]
        public async Task<ActionResult<bool>> DeleteShoppingItem([FromQuery] string userId, [FromQuery] string productId)
        {
            var currentUserId = User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return NotFound();
            }

            if (currentUserId != userId)
            {
                return Unauthorized();
            }

            var item = await db.ShoppingItems.FindAsync(userId, productId);

            if (item == null)
            {
                return NotFound();
            }

            db.ShoppingItems.Remove(item);

            await db.SaveChangesAsync();

            return Ok(true);
        }
    }
}
