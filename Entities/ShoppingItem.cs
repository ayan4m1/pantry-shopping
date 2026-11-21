namespace Shopping.Entities
{
    public enum ShoppingItemStatus { Open, Purchased, Dismissed }

    public class ShoppingItem
    {
        public Guid Id { get; set; }                       // also the restock idempotency key
        public Guid HouseholdId { get; set; }
        public Guid ProductId { get; set; }                // Inventory's product id; no FK across services
        public required string ProductName { get; set; }  // snapshot, max 100
        public string? Unit { get; set; }                  // snapshot, max 20
        public decimal Quantity { get; set; }              // amount to buy, numeric(10,2)
        public ShoppingItemStatus Status { get; set; }     // stored as string
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }   // purchased or dismissed
        public string? CompletedByUserId { get; set; }     // Keycloak sub
        public decimal? PurchasedQuantity { get; set; }
        public DateTimeOffset? RestockedAt { get; set; }   // set when Inventory returns 200
    }
}
