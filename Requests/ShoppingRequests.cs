namespace Shopping.Requests
{
    public record CreateShoppingItemRequest(Guid ProductId, Guid HouseholdId);
}
