namespace pantry_shopping.Entities
{
    public class ShoppingItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public required bool Purchased { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public Product Product { get; set; } = null!;
    }
}
