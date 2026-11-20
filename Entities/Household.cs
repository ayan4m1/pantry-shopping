namespace Shopping.Entities
{
    public class Household
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public List<ShoppingItem> ShoppingItems { get; set; } = [];
    }
}
