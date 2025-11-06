using Microsoft.EntityFrameworkCore;
using SafeFutureInventorySystem.Models;

namespace SafeFutureInventorySystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<InventoryAdjustmentLog> AdjustmentLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<InventoryAdjustmentLog>()
                .HasOne<InventoryItem>()
                .WithMany()
                .HasForeignKey(l => l.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed some initial data
            modelBuilder.Entity<InventoryItem>().HasData(
                new InventoryItem
                {
                    Id = 1,
                    Name = "Canned Beans",
                    Description = "Black beans 15oz can",
                    Quantity = 50,
                    Barcode = "012345678901",
                    ExpirationDate = DateTime.Now.AddDays(365),
                    DateAdded = DateTime.Now,
                    Category = "Food"
                },
                new InventoryItem
                {
                    Id = 2,
                    Name = "Bottled Water",
                    Description = "24-pack bottled water",
                    Quantity = 100,
                    DateAdded = DateTime.Now,
                    Category = "Beverages"
                },
                new InventoryItem
                {
                    Id = 3,
                    Name = "First Aid Kit",
                    Description = "Basic first aid supplies",
                    Quantity = 15,
                    ExpirationDate = DateTime.Now.AddDays(7),
                    DateAdded = DateTime.Now,
                    Category = "Medical"
                }
            );
        }
    }
}
