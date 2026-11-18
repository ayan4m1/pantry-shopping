namespace pantry_shopping.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public List<ShoppingItem> ShoppingItems { get; set; } = [];
    }
}
