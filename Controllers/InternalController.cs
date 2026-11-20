using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping.Controllers
{
    // Service-to-service endpoints; the gateway blocks /api/{service}/internal/** from the outside.
    [ApiController]
    [Route("internal")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InternalController(ShoppingDbContext db) : ControllerBase
    {
        [HttpDelete("household/{householdId:guid}/items")]
        public async Task<IActionResult> DeleteHouseholdItems(Guid householdId)
        {
            await db.ShoppingItems
                .Where((item) => item.HouseholdId == householdId)
                .ExecuteDeleteAsync();

            return NoContent();
        }
    }
}
