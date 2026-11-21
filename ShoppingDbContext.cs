using Microsoft.EntityFrameworkCore;
using Shopping.Entities;

namespace Shopping
{
    public class ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : DbContext(options)
    {
        public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<ShoppingItem>(e =>
            {
                e.HasKey(i => i.Id);
                e.Property(i => i.Id).ValueGeneratedNever();          // set in code: it doubles as the restock idempotency key

                e.Property(i => i.ProductName).HasMaxLength(100).IsRequired();
                e.Property(i => i.Unit).HasMaxLength(20);
                e.Property(i => i.CompletedByUserId).HasMaxLength(64);

                e.Property(i => i.Quantity).HasPrecision(10, 2);
                e.Property(i => i.PurchasedQuantity).HasPrecision(10, 2);

                e.Property(i => i.Status).HasConversion<string>().HasMaxLength(16);

                e.HasIndex(i => new { i.HouseholdId, i.Status });  // listing a household's open items

                e.HasIndex(i => i.ProductId)
                 .IsUnique()
                 .HasFilter("status = 'Open'");                      // at most one open item per product

                e.HasIndex(i => i.RestockedAt)
                 .HasFilter("status = 'Purchased' AND restocked_at IS NULL");   // small index for the retry job

                e.ToTable(t => t.HasCheckConstraint("ck_shopping_items_quantity_positive", "quantity > 0"));
            });
        }
    }
}
