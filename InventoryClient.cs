using Shopping.Requests;

namespace Shopping
{
    public interface IInventoryClient
    {
        Task RestockProductAsync(Guid productId, Guid shoppingItemId, decimal quantity, CancellationToken ct);
    }

    public class InventoryClient(HttpClient http) : IInventoryClient
    {
        public async Task RestockProductAsync(Guid productId, Guid shoppingItemId, decimal quantity, CancellationToken ct)
        {
            using var content = JsonContent.Create(new RestockRequest(shoppingItemId, quantity));
            using var response = await http.PostAsync($"internal/products/{productId}/restock", content, ct);

            response.EnsureSuccessStatusCode();
        }
    }
}
