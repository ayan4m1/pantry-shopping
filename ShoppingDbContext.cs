using Microsoft.EntityFrameworkCore;
using Shopping.Entities;

namespace Shopping
{
    public class ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Product>(e =>
            {
                e.Property(p => p.Name).HasMaxLength(100);
            });

            b.Entity<ShoppingItem>(e =>
            {
                e.HasIndex(si => si.UserId); // for listing all items bound to a user
                e.HasIndex(si => new { si.UserId, si.ProductId }).IsUnique(); // only one product per user
                e.HasIndex(si => si.Purchased);
            });
        }
    }
}
