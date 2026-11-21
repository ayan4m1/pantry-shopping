using Shopping.Entities;

namespace Shopping.Responses
{
    public record ShoppingItemResponse(
        Guid Id,
        Guid HouseholdId,
        Guid ProductId,
        string ProductName,
        string? Unit,
        decimal Quantity,
        ShoppingItemStatus Status,
        DateTimeOffset CreatedAt,
        DateTimeOffset? CompletedAt,
        decimal? PurchasedQuantity
    );
}
