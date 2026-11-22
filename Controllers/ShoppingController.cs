using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Entities;
using Shopping.Requests;
using Shopping.Services;
using System.Security.Claims;

namespace Shopping.Controllers
{
    [ApiController]
    [Route("[controller]/households/{householdId:guid}/shopping-items/{itemId:guid}")]
    [Authorize(Policy = Policies.HouseholdMember)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class ShoppingController(ShoppingDbContext db) : ControllerBase
    {
        [HttpPost("purchase")]
        [EndpointName("purchaseShoppingItem")]
        [ProducesResponseType<bool>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> Purchase(Guid householdId, Guid itemId, [FromBody] PurchaseRequest request)
        {
            var userId = User.FindFirstValue("sub")!;

            var shoppingItem = await db.ShoppingItems.FindAsync(itemId);

            if (shoppingItem == null || shoppingItem.HouseholdId != householdId)
            {
                return NotFound();
            }

            shoppingItem.PurchasedQuantity = request.Quantity;
            shoppingItem.CompletedByUserId = userId;
            shoppingItem.Status = ShoppingItemStatus.Purchased;

            await db.SaveChangesAsync();

            return Ok(true);
        }

        [HttpDelete("dismiss")]
        [EndpointName("dismissShoppingItem")]
        [ProducesResponseType<bool>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> Dismiss(Guid householdId, Guid itemId)
        {
            var userId = User.FindFirstValue("sub")!;

            var shoppingItem = await db.ShoppingItems.FindAsync(itemId);

            if (shoppingItem == null || shoppingItem.HouseholdId != householdId)
            {
                return NotFound();
            }

            shoppingItem.CompletedByUserId = userId;
            shoppingItem.Status = ShoppingItemStatus.Dismissed;

            await db.SaveChangesAsync();

            return Ok(true);
        }
    }
}
