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
                     Name = "Pampers Newborn Diapers",
                     Description = "Size N, Up to 10 lbs, 32 count pack",
                     Quantity = 150,
                     Barcode = "037000465911",
                     ExpirationDate = DateTime.Now.AddMonths(18),
                     DateAdded = DateTime.Now.AddDays(-30),
                     Category = "Diapers"
                 },
                new InventoryItem
                {
                    Id = 2,
                    Name = "Huggies Size 1 Diapers",
                    Description = "Size 1, 8-14 lbs, 84 count pack",
                    Quantity = 200,
                    Barcode = "036000406481",
                    ExpirationDate = DateTime.Now.AddMonths(24),
                    DateAdded = DateTime.Now.AddDays(-25),
                    Category = "Diapers"
                },
                new InventoryItem
                {
                    Id = 3,
                    Name = "Pampers Size 2 Diapers",
                    Description = "Size 2, 12-18 lbs, 112 count pack",
                    Quantity = 180,
                    Barcode = "037000465928",
                    ExpirationDate = DateTime.Now.AddMonths(20),
                    DateAdded = DateTime.Now.AddDays(-20),
                    Category = "Diapers"
                },
                new InventoryItem
                {
                    Id = 4,
                    Name = "Huggies Size 3 Diapers",
                    Description = "Size 3, 16-28 lbs, 104 count pack",
                    Quantity = 165,
                    Barcode = "036000406498",
                    ExpirationDate = DateTime.Now.AddMonths(22),
                    DateAdded = DateTime.Now.AddDays(-15),
                    Category = "Diapers"
                },
                new InventoryItem
                {
                    Id = 5,
                    Name = "Pampers Size 4 Diapers",
                    Description = "Size 4, 22-37 lbs, 92 count pack",
                    Quantity = 140,
                    Barcode = "037000465935",
                    ExpirationDate = DateTime.Now.AddMonths(19),
                    DateAdded = DateTime.Now.AddDays(-10),
                    Category = "Diapers"
                },
                new InventoryItem
                {
                    Id = 6,
                    Name = "Huggies Size 5 Diapers",
                    Description = "Size 5, 27+ lbs, 80 count pack",
                    Quantity = 120,
                    Barcode = "036000406504",
                    ExpirationDate = DateTime.Now.AddMonths(21),
                    DateAdded = DateTime.Now.AddDays(-8),
                    Category = "Diapers"
                },
                new InventoryItem
                {
                    Id = 7,
                    Name = "Pampers Size 6 Diapers",
                    Description = "Size 6, 35+ lbs, 68 count pack",
                    Quantity = 95,
                    Barcode = "037000465942",
                    ExpirationDate = DateTime.Now.AddMonths(23),
                    DateAdded = DateTime.Now.AddDays(-5),
                    Category = "Diapers"
                },
                new InventoryItem
                {
                    Id = 8,
                    Name = "Seventh Generation Size 1 Diapers",
                    Description = "Eco-friendly, Size 1, 8-14 lbs, 40 count",
                    Quantity = 75,
                    Barcode = "732913441914",
                    ExpirationDate = DateTime.Now.AddMonths(15),
                    DateAdded = DateTime.Now.AddDays(-12),
                    Category = "Diapers"
                },

                // BABY WIPES
                new InventoryItem
                {
                    Id = 9,
                    Name = "Pampers Sensitive Baby Wipes",
                    Description = "Hypoallergenic, 504 count (7 packs)",
                    Quantity = 250,
                    Barcode = "037000830689",
                    ExpirationDate = DateTime.Now.AddMonths(12),
                    DateAdded = DateTime.Now.AddDays(-18),
                    Category = "Baby Wipes"
                },
                new InventoryItem
                {
                    Id = 10,
                    Name = "Huggies Natural Care Baby Wipes",
                    Description = "Fragrance-free, 552 count (8 packs)",
                    Quantity = 220,
                    Barcode = "036000516500",
                    ExpirationDate = DateTime.Now.AddMonths(14),
                    DateAdded = DateTime.Now.AddDays(-22),
                    Category = "Baby Wipes"
                },
                new InventoryItem
                {
                    Id = 11,
                    Name = "WaterWipes Biodegradable Baby Wipes",
                    Description = "99.9% water, 540 count (9 packs)",
                    Quantity = 180,
                    Barcode = "859668006010",
                    ExpirationDate = DateTime.Now.AddDays(5),
                    DateAdded = DateTime.Now.AddDays(-40),
                    Category = "Baby Wipes"
                },

                // DIAPERING ESSENTIALS
                new InventoryItem
                {
                    Id = 12,
                    Name = "Diaper Rash Cream",
                    Description = "Desitin Maximum Strength, 4 oz tube",
                    Quantity = 85,
                    Barcode = "067981105001",
                    ExpirationDate = DateTime.Now.AddMonths(18),
                    DateAdded = DateTime.Now.AddDays(-14),
                    Category = "Diapering"
                },
                new InventoryItem
                {
                    Id = 13,
                    Name = "Baby Powder",
                    Description = "Johnson's Baby Powder, cornstarch, 9 oz",
                    Quantity = 110,
                    Barcode = "381371161416",
                    ExpirationDate = DateTime.Now.AddMonths(24),
                    DateAdded = DateTime.Now.AddDays(-19),
                    Category = "Baby Care"
                },
                new InventoryItem
                {
                    Id = 14,
                    Name = "Disposable Changing Pads",
                    Description = "Waterproof, 50 count pack",
                    Quantity = 65,
                    Barcode = "735363012010",
                    DateAdded = DateTime.Now.AddDays(-7),
                    Category = "Diapering"
                },

                // BABY FORMULA
                new InventoryItem
                {
                    Id = 15,
                    Name = "Similac Infant Formula",
                    Description = "Pro-Advance, 30.8 oz powder container",
                    Quantity = 130,
                    Barcode = "070074682532",
                    ExpirationDate = DateTime.Now.AddMonths(9),
                    DateAdded = DateTime.Now.AddDays(-35),
                    Category = "Baby Formula"
                },
                new InventoryItem
                {
                    Id = 16,
                    Name = "Enfamil NeuroPro Baby Formula",
                    Description = "Infant formula powder, 28.3 oz",
                    Quantity = 145,
                    Barcode = "300871214415",
                    ExpirationDate = DateTime.Now.AddMonths(10),
                    DateAdded = DateTime.Now.AddDays(-28),
                    Category = "Baby Formula"
                },
                new InventoryItem
                {
                    Id = 17,
                    Name = "Gerber Good Start Gentle Formula",
                    Description = "Non-GMO powder, 32 oz container",
                    Quantity = 95,
                    Barcode = "050000339877",
                    ExpirationDate = DateTime.Now.AddDays(3),
                    DateAdded = DateTime.Now.AddDays(-45),
                    Category = "Baby Formula"
                },

                // BABY BOTTLES & FEEDING
                new InventoryItem
                {
                    Id = 18,
                    Name = "Baby Bottles 8oz",
                    Description = "Dr. Brown's Anti-Colic, 3-pack",
                    Quantity = 70,
                    Barcode = "072239311004",
                    DateAdded = DateTime.Now.AddDays(-11),
                    Category = "Feeding"
                },
                new InventoryItem
                {
                    Id = 19,
                    Name = "Bottle Nipples - Slow Flow",
                    Description = "Silicone, 6-pack, 0-3 months",
                    Quantity = 125,
                    Barcode = "072239314012",
                    DateAdded = DateTime.Now.AddDays(-16),
                    Category = "Feeding"
                },
                new InventoryItem
                {
                    Id = 20,
                    Name = "Burp Cloths",
                    Description = "100% cotton, 10-pack set",
                    Quantity = 90,
                    Barcode = "849854012345",
                    DateAdded = DateTime.Now.AddDays(-9),
                    Category = "Feeding"
                },

                // BABY CLOTHING
                new InventoryItem
                {
                    Id = 21,
                    Name = "Onesies 0-3 Months",
                    Description = "Short sleeve, 5-pack, assorted colors",
                    Quantity = 160,
                    Barcode = "078742317298",
                    DateAdded = DateTime.Now.AddDays(-21),
                    Category = "Clothing"
                },
                new InventoryItem
                {
                    Id = 22,
                    Name = "Baby Sleepers 3-6 Months",
                    Description = "Footed sleepers, fleece, 3-pack",
                    Quantity = 105,
                    Barcode = "078742317305",
                    DateAdded = DateTime.Now.AddDays(-13),
                    Category = "Clothing"
                },
                new InventoryItem
                {
                    Id = 23,
                    Name = "Baby Socks 0-12 Months",
                    Description = "Non-slip grip, 12-pack",
                    Quantity = 200,
                    Barcode = "849854023456",
                    DateAdded = DateTime.Now.AddDays(-6),
                    Category = "Clothing"
                },
                new InventoryItem
                {
                    Id = 24,
                    Name = "Baby Mittens",
                    Description = "Scratch-free mittens, 6-pack",
                    Quantity = 80,
                    Barcode = "849854034567",
                    DateAdded = DateTime.Now.AddDays(-24),
                    Category = "Clothing"
                },

                // BABY CARE & HYGIENE
                new InventoryItem
                {
                    Id = 25,
                    Name = "Baby Shampoo & Body Wash",
                    Description = "Johnson's Head-to-Toe, 27.1 fl oz",
                    Quantity = 135,
                    Barcode = "381371161423",
                    ExpirationDate = DateTime.Now.AddMonths(30),
                    DateAdded = DateTime.Now.AddDays(-17),
                    Category = "Baby Care"
                },
                new InventoryItem
                {
                    Id = 26,
                    Name = "Baby Lotion",
                    Description = "Aveeno Baby Daily Moisture, 18 fl oz",
                    Quantity = 115,
                    Barcode = "381371161430",
                    ExpirationDate = DateTime.Now.AddMonths(26),
                    DateAdded = DateTime.Now.AddDays(-23),
                    Category = "Baby Care"
                },
                new InventoryItem
                {
                    Id = 27,
                    Name = "Baby Nail Clippers",
                    Description = "Safety nail care set with file",
                    Quantity = 55,
                    Barcode = "849854045678",
                    DateAdded = DateTime.Now.AddDays(-4),
                    Category = "Baby Care"
                },
                new InventoryItem
                {
                    Id = 28,
                    Name = "Nasal Aspirator",
                    Description = "NoseFrida with 20 filters",
                    Quantity = 45,
                    Barcode = "853689006133",
                    DateAdded = DateTime.Now.AddDays(-27),
                    Category = "Baby Health"
                },

                // BABY BEDDING & COMFORT
                new InventoryItem
                {
                    Id = 29,
                    Name = "Swaddle Blankets",
                    Description = "Muslin, 4-pack, 47x47 inches",
                    Quantity = 100,
                    Barcode = "849854056789",
                    DateAdded = DateTime.Now.AddDays(-26),
                    Category = "Bedding"
                },
                new InventoryItem
                {
                    Id = 30,
                    Name = "Crib Sheets",
                    Description = "Fitted, 100% cotton, 2-pack",
                    Quantity = 75,
                    Barcode = "849854067890",
                    DateAdded = DateTime.Now.AddDays(-15),
                    Category = "Bedding"
                },
                new InventoryItem
                {
                    Id = 31,
                    Name = "Baby Pacifiers 0-6 Months",
                    Description = "Orthodontic, BPA-free, 4-pack",
                    Quantity = 140,
                    Barcode = "072239314029",
                    ExpirationDate = DateTime.Now.AddMonths(36),
                    DateAdded = DateTime.Now.AddDays(-8),
                    Category = "Comfort"
                },

                // BABY BATHING
                new InventoryItem
                {
                    Id = 32,
                    Name = "Baby Hooded Towels",
                    Description = "Extra soft, 30x30 inches, 3-pack",
                    Quantity = 88,
                    Barcode = "849854078901",
                    DateAdded = DateTime.Now.AddDays(-12),
                    Category = "Bathing"
                },
                new InventoryItem
                {
                    Id = 33,
                    Name = "Baby Washcloths",
                    Description = "Ultra-soft, 12-pack",
                    Quantity = 150,
                    Barcode = "849854089012",
                    DateAdded = DateTime.Now.AddDays(-29),
                    Category = "Bathing"
                },

                // ADDITIONAL ITEMS
                new InventoryItem
                {
                    Id = 34,
                    Name = "Diaper Bags",
                    Description = "Multi-pocket backpack style",
                    Quantity = 35,
                    Barcode = "849854090123",
                    DateAdded = DateTime.Now.AddDays(-31),
                    Category = "Accessories"
                },
                new InventoryItem
                {
                    Id = 35,
                    Name = "Baby Thermometer",
                    Description = "Digital forehead & ear thermometer",
                    Quantity = 50,
                    Barcode = "849854101234",
                    DateAdded = DateTime.Now.AddDays(-33),
                    Category = "Baby Health"
                }
            );
        }
    }
}
