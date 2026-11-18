using Microsoft.EntityFrameworkCore;
using pantry_shopping.Entities;

namespace pantry_shopping
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

            b.Entity<ShoppingItem>();
        }
    }
}
