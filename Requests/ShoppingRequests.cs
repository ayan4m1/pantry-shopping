namespace Shopping.Requests
{
    public record CreateShoppingItemRequest(Guid HouseholdId, Guid ProductId, string ProductName, string? Unit, decimal Quantity);
    public record PurchaseRequest(decimal? Quantity);
    public record RestockRequest(Guid ShoppingItemId, decimal Quantity);
}
