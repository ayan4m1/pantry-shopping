namespace Shopping.Entities
{
    public class ShoppingItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid HouseholdId { get; set; }
        public required string UserId { get; set; } // Keycloak "sub"
        public required bool Purchased { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public Product Product { get; set; } = null!;
        public Household Household { get; set; } = null!;
    }
}
