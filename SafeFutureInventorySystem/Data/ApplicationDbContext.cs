using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SafeFutureInventorySystem.Models;
using System.Configuration;

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
        public DbSet<DonationLog> DonationLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<InventoryAdjustmentLog>()
                .HasOne<InventoryItem>()
                .WithMany(i => i.AdjustmentLogs)
                .HasForeignKey(l => l.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DonationLog>()
                .HasOne(d => d.InventoryItem)
                .WithMany(i => i.DonationLogs)
                .HasForeignKey(d => d.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================================================
            // INVENTORY ITEMS SEED DATA
            // NOTE: DonorName/Phone/Email removed — now tracked per-donation
            //       Quantities reflect the SUM of all DonationLog entries
            // ============================================================
            modelBuilder.Entity<InventoryItem>().HasData(

                // ============================================
                // EXPIRED ITEMS
                // ============================================
                new InventoryItem
                {
                    Id = 1,
                    Name = "Expired Baby Formula",
                    Description = "EXPIRED - Similac Advance, 12.4 oz",
                    Quantity = 15,
                    Barcode = "070074680002",
                    ExpirationDate = DateTime.Now.AddDays(-30),
                    DateAdded = DateTime.Now.AddDays(-120),
                    Category = "Baby Formula"
                },
                new InventoryItem
                {
                    Id = 2,
                    Name = "Expired Baby Wipes",
                    Description = "EXPIRED - Huggies Natural Care, 72 count",
                    Quantity = 8,
                    Barcode = "036000516517",
                    ExpirationDate = DateTime.Now.AddDays(-15),
                    DateAdded = DateTime.Now.AddDays(-90),
                    Category = "Baby Wipes"
                },
                new InventoryItem
                {
                    Id = 3,
                    Name = "Expired Diaper Cream",
                    Description = "EXPIRED - Boudreaux's Butt Paste, 4 oz",
                    Quantity = 3,
                    Barcode = "085898800015",
                    ExpirationDate = DateTime.Now.AddDays(-60),
                    DateAdded = DateTime.Now.AddDays(-150),
                    Category = "Baby Care"
                },

                // ============================================
                // EXPIRING SOON
                // ============================================
                new InventoryItem
                {
                    Id = 4,
                    Name = "Gerber Good Start Formula",
                    Description = "EXPIRING SOON - Gentle powder, 32 oz",
                    Quantity = 45,
                    Barcode = "050000339877",
                    ExpirationDate = DateTime.Now.AddDays(3),
                    DateAdded = DateTime.Now.AddDays(-60),
                    Category = "Baby Formula"
                },
                new InventoryItem
                {
                    Id = 5,
                    Name = "WaterWipes Baby Wipes",
                    Description = "EXPIRING SOON - 99.9% water, 540 count",
                    Quantity = 180,
                    Barcode = "859668006010",
                    ExpirationDate = DateTime.Now.AddDays(5),
                    DateAdded = DateTime.Now.AddDays(-40),
                    Category = "Baby Wipes"
                },
                new InventoryItem
                {
                    Id = 6,
                    Name = "Organic Baby Food - Peas",
                    Description = "EXPIRING SOON - Stage 1, 4 oz jar",
                    Quantity = 22,
                    Barcode = "051000138255",
                    ExpirationDate = DateTime.Now.AddDays(7),
                    DateAdded = DateTime.Now.AddDays(-50),
                    Category = "Baby Food"
                },

                // ============================================
                // EXPIRING THIS MONTH
                // ============================================
                new InventoryItem
                {
                    Id = 7,
                    Name = "Similac Pro-Advance Formula",
                    Description = "Expiring this month - 30.8 oz powder",
                    Quantity = 130,
                    Barcode = "070074682532",
                    ExpirationDate = DateTime.Now.AddDays(25),
                    DateAdded = DateTime.Now.AddDays(-35),
                    Category = "Baby Formula"
                },
                new InventoryItem
                {
                    Id = 8,
                    Name = "Pampers Sensitive Wipes",
                    Description = "Expiring this month - 504 count",
                    Quantity = 250,
                    Barcode = "037000830689",
                    ExpirationDate = DateTime.Now.AddDays(20),
                    DateAdded = DateTime.Now.AddDays(-18),
                    Category = "Baby Wipes"
                },
                new InventoryItem
                {
                    Id = 9,
                    Name = "Baby Vitamin D Drops",
                    Description = "Expiring this month - Liquid supplement",
                    Quantity = 68,
                    Barcode = "363824009001",
                    ExpirationDate = DateTime.Now.AddDays(28),
                    DateAdded = DateTime.Now.AddDays(-45),
                    Category = "Baby Health"
                },

                // ============================================
                // GOOD CONDITION
                // ============================================
                new InventoryItem
                {
                    Id = 10,
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
                    Id = 11,
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
                    Id = 12,
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
                    Id = 13,
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
                    Id = 14,
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
                    Id = 15,
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
                    Id = 16,
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
                    Id = 17,
                    Name = "Enfamil NeuroPro Formula",
                    Description = "Infant formula powder, 28.3 oz",
                    Quantity = 145,
                    Barcode = "300871214415",
                    ExpirationDate = DateTime.Now.AddMonths(10),
                    DateAdded = DateTime.Now.AddDays(-28),
                    Category = "Baby Formula"
                },
                new InventoryItem
                {
                    Id = 18,
                    Name = "Huggies Natural Care Wipes",
                    Description = "Fragrance-free, 552 count (8 packs)",
                    Quantity = 220,
                    Barcode = "036000516500",
                    ExpirationDate = DateTime.Now.AddMonths(14),
                    DateAdded = DateTime.Now.AddDays(-22),
                    Category = "Baby Wipes"
                },
                new InventoryItem
                {
                    Id = 19,
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
                    Id = 20,
                    Name = "Baby Shampoo & Body Wash",
                    Description = "Johnson's Head-to-Toe, 27.1 fl oz",
                    Quantity = 135,
                    Barcode = "381371161423",
                    ExpirationDate = DateTime.Now.AddMonths(30),
                    DateAdded = DateTime.Now.AddDays(-17),
                    Category = "Baby Care"
                },

                // ============================================
                // NO EXPIRATION DATE
                // ============================================
                new InventoryItem
                {
                    Id = 21,
                    Name = "Baby Bottles 8oz",
                    Description = "Dr. Brown's Anti-Colic, 3-pack",
                    Quantity = 70,
                    Barcode = "072239311004",
                    DateAdded = DateTime.Now.AddDays(-11),
                    Category = "Feeding"
                },
                new InventoryItem
                {
                    Id = 22,
                    Name = "Bottle Nipples - Slow Flow",
                    Description = "Silicone, 6-pack, 0-3 months",
                    Quantity = 125,
                    Barcode = "072239314012",
                    DateAdded = DateTime.Now.AddDays(-16),
                    Category = "Feeding"
                },
                new InventoryItem
                {
                    Id = 23,
                    Name = "Burp Cloths",
                    Description = "100% cotton, 10-pack set",
                    Quantity = 90,
                    Barcode = "849854012345",
                    DateAdded = DateTime.Now.AddDays(-9),
                    Category = "Feeding"
                },
                new InventoryItem
                {
                    Id = 24,
                    Name = "Onesies 0-3 Months",
                    Description = "Short sleeve, 5-pack, assorted colors",
                    Quantity = 160,
                    Barcode = "078742317298",
                    DateAdded = DateTime.Now.AddDays(-21),
                    Category = "Clothing"
                },
                new InventoryItem
                {
                    Id = 25,
                    Name = "Baby Sleepers 3-6 Months",
                    Description = "Footed sleepers, fleece, 3-pack",
                    Quantity = 105,
                    Barcode = "078742317305",
                    DateAdded = DateTime.Now.AddDays(-13),
                    Category = "Clothing"
                },
                new InventoryItem
                {
                    Id = 26,
                    Name = "Baby Socks 0-12 Months",
                    Description = "Non-slip grip, 12-pack",
                    Quantity = 200,
                    Barcode = "849854023456",
                    DateAdded = DateTime.Now.AddDays(-6),
                    Category = "Clothing"
                },
                new InventoryItem
                {
                    Id = 27,
                    Name = "Baby Mittens",
                    Description = "Scratch-free mittens, 6-pack",
                    Quantity = 80,
                    Barcode = "849854034567",
                    DateAdded = DateTime.Now.AddDays(-24),
                    Category = "Clothing"
                },
                new InventoryItem
                {
                    Id = 28,
                    Name = "Disposable Changing Pads",
                    Description = "Waterproof, 50 count pack",
                    Quantity = 65,
                    Barcode = "735363012010",
                    DateAdded = DateTime.Now.AddDays(-7),
                    Category = "Diapering"
                },
                new InventoryItem
                {
                    Id = 29,
                    Name = "Baby Nail Clippers",
                    Description = "Safety nail care set with file",
                    Quantity = 55,
                    Barcode = "849854045678",
                    DateAdded = DateTime.Now.AddDays(-4),
                    Category = "Baby Care"
                },
                new InventoryItem
                {
                    Id = 30,
                    Name = "Nasal Aspirator",
                    Description = "NoseFrida with 20 filters",
                    Quantity = 45,
                    Barcode = "853689006133",
                    DateAdded = DateTime.Now.AddDays(-27),
                    Category = "Baby Health"
                },
                new InventoryItem
                {
                    Id = 31,
                    Name = "Swaddle Blankets",
                    Description = "Muslin, 4-pack, 47x47 inches",
                    Quantity = 100,
                    Barcode = "849854056789",
                    DateAdded = DateTime.Now.AddDays(-26),
                    Category = "Bedding"
                },
                new InventoryItem
                {
                    Id = 32,
                    Name = "Crib Sheets",
                    Description = "Fitted, 100% cotton, 2-pack",
                    Quantity = 75,
                    Barcode = "849854067890",
                    DateAdded = DateTime.Now.AddDays(-15),
                    Category = "Bedding"
                },
                new InventoryItem
                {
                    Id = 33,
                    Name = "Baby Hooded Towels",
                    Description = "Extra soft, 30x30 inches, 3-pack",
                    Quantity = 88,
                    Barcode = "849854078901",
                    DateAdded = DateTime.Now.AddDays(-12),
                    Category = "Bathing"
                },
                new InventoryItem
                {
                    Id = 34,
                    Name = "Baby Washcloths",
                    Description = "Ultra-soft, 12-pack",
                    Quantity = 150,
                    Barcode = "849854089012",
                    DateAdded = DateTime.Now.AddDays(-29),
                    Category = "Bathing"
                },
                new InventoryItem
                {
                    Id = 35,
                    Name = "Diaper Bags",
                    Description = "Multi-pocket backpack style",
                    Quantity = 35,
                    Barcode = "849854090123",
                    DateAdded = DateTime.Now.AddDays(-31),
                    Category = "Accessories"
                },
                new InventoryItem
                {
                    Id = 36,
                    Name = "Baby Thermometer",
                    Description = "Digital forehead & ear thermometer",
                    Quantity = 50,
                    Barcode = "849854101234",
                    DateAdded = DateTime.Now.AddDays(-33),
                    Category = "Baby Health"
                },

                // ============================================
                // ADDITIONAL ITEMS (37-60)
                // ============================================
                new InventoryItem
                {
                    Id = 37,
                    Name = "Seventh Generation Size 1 Diapers",
                    Description = "Eco-friendly, Size 1, 8-14 lbs, 40 count",
                    Quantity = 75,
                    Barcode = "732913441914",
                    ExpirationDate = DateTime.Now.AddMonths(15),
                    DateAdded = DateTime.Now.AddDays(-12),
                    Category = "Diapers"
                },
                new InventoryItem
                {
                    Id = 38,
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
                    Id = 39,
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
                    Id = 40,
                    Name = "Baby Pacifiers 0-6 Months",
                    Description = "Orthodontic, BPA-free, 4-pack",
                    Quantity = 140,
                    Barcode = "072239314029",
                    ExpirationDate = DateTime.Now.AddMonths(36),
                    DateAdded = DateTime.Now.AddDays(-8),
                    Category = "Comfort"
                },
                new InventoryItem
                {
                    Id = 41,
                    Name = "Baby Bibs - Waterproof",
                    Description = "Silicone, easy clean, 3-pack",
                    Quantity = 92,
                    Barcode = "849854112345",
                    DateAdded = DateTime.Now.AddDays(-42),
                    Category = "Feeding"
                },
                new InventoryItem
                {
                    Id = 42,
                    Name = "Baby Food - Sweet Potatoes",
                    Description = "Organic stage 1, 4 oz jar",
                    Quantity = 185,
                    Barcode = "051000138262",
                    ExpirationDate = DateTime.Now.AddDays(45),
                    DateAdded = DateTime.Now.AddDays(-55),
                    Category = "Baby Food"
                },
                new InventoryItem
                {
                    Id = 43,
                    Name = "Baby Food - Carrots",
                    Description = "Organic stage 1, 4 oz jar",
                    Quantity = 167,
                    Barcode = "051000138279",
                    ExpirationDate = DateTime.Now.AddDays(50),
                    DateAdded = DateTime.Now.AddDays(-48),
                    Category = "Baby Food"
                },
                new InventoryItem
                {
                    Id = 44,
                    Name = "Baby Spoons",
                    Description = "Soft-tip silicone, 6-pack",
                    Quantity = 78,
                    Barcode = "849854123456",
                    DateAdded = DateTime.Now.AddDays(-38),
                    Category = "Feeding"
                },
                new InventoryItem
                {
                    Id = 45,
                    Name = "Baby Bowls with Suction",
                    Description = "BPA-free, 3-pack with lids",
                    Quantity = 61,
                    Barcode = "849854134567",
                    DateAdded = DateTime.Now.AddDays(-52),
                    Category = "Feeding"
                },
                new InventoryItem
                {
                    Id = 46,
                    Name = "Teething Toys",
                    Description = "BPA-free silicone, 4-pack",
                    Quantity = 103,
                    Barcode = "849854145678",
                    DateAdded = DateTime.Now.AddDays(-44),
                    Category = "Baby Health"
                },
                new InventoryItem
                {
                    Id = 47,
                    Name = "Baby Sunscreen SPF 50",
                    Description = "Mineral-based, tear-free, 3 oz",
                    Quantity = 47,
                    Barcode = "738443002151",
                    ExpirationDate = DateTime.Now.AddMonths(16),
                    DateAdded = DateTime.Now.AddDays(-65),
                    Category = "Baby Care"
                },
                new InventoryItem
                {
                    Id = 48,
                    Name = "Baby Laundry Detergent",
                    Description = "Dreft, hypoallergenic, 50 oz",
                    Quantity = 82,
                    Barcode = "037000120261",
                    DateAdded = DateTime.Now.AddDays(-71),
                    Category = "Baby Care"
                },
                new InventoryItem
                {
                    Id = 49,
                    Name = "Baby Safety Gates",
                    Description = "Pressure-mounted, 29-38 inches",
                    Quantity = 18,
                    Barcode = "849854156789",
                    DateAdded = DateTime.Now.AddDays(-88),
                    Category = "Safety"
                },
                new InventoryItem
                {
                    Id = 50,
                    Name = "Outlet Covers",
                    Description = "Child-proof, 36-pack",
                    Quantity = 234,
                    Barcode = "849854167890",
                    DateAdded = DateTime.Now.AddDays(-77),
                    Category = "Safety"
                },
                new InventoryItem
                {
                    Id = 51,
                    Name = "Baby Monitor",
                    Description = "Video monitor with night vision",
                    Quantity = 12,
                    Barcode = "849854178901",
                    DateAdded = DateTime.Now.AddDays(-95),
                    Category = "Safety"
                },
                new InventoryItem
                {
                    Id = 52,
                    Name = "Baby Humidifier",
                    Description = "Cool mist, 1 gallon capacity",
                    Quantity = 28,
                    Barcode = "849854189012",
                    DateAdded = DateTime.Now.AddDays(-102),
                    Category = "Baby Health"
                },
                new InventoryItem
                {
                    Id = 53,
                    Name = "Nursing Pads",
                    Description = "Disposable, 60 count box",
                    Quantity = 144,
                    Barcode = "849854190123",
                    DateAdded = DateTime.Now.AddDays(-58),
                    Category = "Feeding"
                },
                new InventoryItem
                {
                    Id = 54,
                    Name = "Baby Bath Tub",
                    Description = "Infant to toddler, with sling",
                    Quantity = 22,
                    Barcode = "849854201234",
                    DateAdded = DateTime.Now.AddDays(-83),
                    Category = "Bathing"
                },
                new InventoryItem
                {
                    Id = 55,
                    Name = "Baby Hair Brush Set",
                    Description = "Soft bristle brush and comb",
                    Quantity = 67,
                    Barcode = "849854212345",
                    DateAdded = DateTime.Now.AddDays(-69),
                    Category = "Baby Care"
                },
                new InventoryItem
                {
                    Id = 56,
                    Name = "Diaper Pail Refills",
                    Description = "Deodorizing, 6-pack rings",
                    Quantity = 98,
                    Barcode = "849854223456",
                    DateAdded = DateTime.Now.AddDays(-46),
                    Category = "Diapering"
                },
                new InventoryItem
                {
                    Id = 57,
                    Name = "Baby Carrier",
                    Description = "Ergonomic, 8-33 lbs capacity",
                    Quantity = 15,
                    Barcode = "849854234567",
                    DateAdded = DateTime.Now.AddDays(-110),
                    Category = "Accessories"
                },
                new InventoryItem
                {
                    Id = 58,
                    Name = "Stroller Rain Cover",
                    Description = "Universal fit, clear PVC",
                    Quantity = 31,
                    Barcode = "849854245678",
                    DateAdded = DateTime.Now.AddDays(-98),
                    Category = "Accessories"
                },
                new InventoryItem
                {
                    Id = 59,
                    Name = "Baby Hangers",
                    Description = "Velvet non-slip, 30-pack",
                    Quantity = 176,
                    Barcode = "849854256789",
                    DateAdded = DateTime.Now.AddDays(-62),
                    Category = "Accessories"
                },
                new InventoryItem
                {
                    Id = 60,
                    Name = "Teething Gel",
                    Description = "Natural, benzocaine-free, 0.33 oz",
                    Quantity = 54,
                    Barcode = "363824012001",
                    ExpirationDate = DateTime.Now.AddDays(2),
                    DateAdded = DateTime.Now.AddDays(-75),
                    Category = "Baby Health"
                }
            );

            // ============================================================
            // DONATION LOG SEED DATA
            //
            // Strategy:
            //  - Every item gets at least 1 DonationLog (the original donor).
            //  - Several high-volume items get 2-3 donations to demonstrate
            //    the merge/accumulation feature is working correctly.
            //  - Quantities across DonationLogs for an item ADD UP to the
            //    item's total Quantity value.
            //
            // Items with multiple donations (to showcase the merge feature):
            //   Id=10 (Pampers Newborn): 3 donors → 60+50+40 = 150
            //   Id=11 (Huggies Size 1):  3 donors → 80+70+50 = 200
            //   Id=17 (Enfamil):         2 donors → 100+45  = 145
            //   Id=18 (Huggies Wipes):   3 donors → 100+80+40 = 220
            //   Id=24 (Onesies 0-3M):    2 donors → 100+60  = 160
            //   Id=42 (Sweet Potatoes):  2 donors → 100+85  = 185
            //   Id=50 (Outlet Covers):   3 donors → 100+84+50 = 234
            //   Id=54 (Baby Bath Tub):   2 donors → 12+10   = 22
            // ============================================================
            modelBuilder.Entity<DonationLog>().HasData(

                // --- Id=1: Expired Baby Formula (15 units, 1 donor) ---
                new DonationLog { Id = 1, InventoryItemId = 1, QuantityDonated = 15, DonationDate = DateTime.Now.AddDays(-120), DonorName = "Sarah Johnson",Notes = "Initial donation — item created." },

                // --- Id=2: Expired Baby Wipes (8 units, 1 donor) ---
                new DonationLog { Id = 2, InventoryItemId = 2, QuantityDonated = 8, DonationDate = DateTime.Now.AddDays(-90), DonorName = "Michael Chen", Notes = "Initial donation — item created." },

                // --- Id=3: Expired Diaper Cream (3 units, 1 donor) ---
                new DonationLog { Id = 3, InventoryItemId = 3, QuantityDonated = 3, DonationDate = DateTime.Now.AddDays(-150), DonorName = "Jennifer Martinez", Notes = "Initial donation — item created." },

                // --- Id=4: Gerber Good Start Formula (45 units, 1 donor) ---
                new DonationLog { Id = 4, InventoryItemId = 4, QuantityDonated = 45, DonationDate = DateTime.Now.AddDays(-60), DonorName = "Robert Williams", Notes = "Initial donation — item created." },

                // --- Id=5: WaterWipes Baby Wipes (180 units, 1 donor) ---
                new DonationLog { Id = 5, InventoryItemId = 5, QuantityDonated = 180, DonationDate = DateTime.Now.AddDays(-40), DonorName = "Lisa Anderson", Notes = "Initial donation — item created." },

                // --- Id=6: Organic Baby Food - Peas (22 units, 1 donor) ---
                new DonationLog { Id = 6, InventoryItemId = 6, QuantityDonated = 22, DonationDate = DateTime.Now.AddDays(-50), DonorName = "Target Store #1234", Notes = "Initial donation — item created." },

                // --- Id=7: Similac Pro-Advance Formula (130 units, 1 donor) ---
                new DonationLog { Id = 7, InventoryItemId = 7, QuantityDonated = 130, DonationDate = DateTime.Now.AddDays(-35), DonorName = "David Thompson", Notes = "Initial donation — item created." },

                // --- Id=8: Pampers Sensitive Wipes (250 units, 1 donor) ---
                new DonationLog { Id = 8, InventoryItemId = 8, QuantityDonated = 250, DonationDate = DateTime.Now.AddDays(-18), DonorName = "Emily Davis",  Notes = "Initial donation — item created." },

                // --- Id=9: Baby Vitamin D Drops (68 units, 1 donor) ---
                new DonationLog { Id = 9, InventoryItemId = 9, QuantityDonated = 68, DonationDate = DateTime.Now.AddDays(-45), DonorName = "James Wilson", Notes = "Initial donation — item created." },

                // --- Id=10: Pampers Newborn Diapers (150 units, 3 donors) ---
                // Demonstrates the merge feature: 60 + 50 + 40 = 150
                new DonationLog { Id = 10, InventoryItemId = 10, QuantityDonated = 60, DonationDate = DateTime.Now.AddDays(-30), DonorName = "Patricia Moore",Notes = "Initial donation — item created." },
                new DonationLog { Id = 11, InventoryItemId = 10, QuantityDonated = 50, DonationDate = DateTime.Now.AddDays(-18), DonorName = "Walmart Store #5678",Notes = "Merged into existing stock. Previous qty: 60" },
                new DonationLog { Id = 12, InventoryItemId = 10, QuantityDonated = 40, DonationDate = DateTime.Now.AddDays(-7), DonorName = "St. Mary's Church", Notes = "Merged into existing stock. Previous qty: 110" },

                // --- Id=11: Huggies Size 1 Diapers (200 units, 3 donors) ---
                // 80 + 70 + 50 = 200
                new DonationLog { Id = 13, InventoryItemId = 11, QuantityDonated = 80, DonationDate = DateTime.Now.AddDays(-25), DonorName = "Christopher Taylor",  Notes = "Initial donation — item created." },
                new DonationLog { Id = 14, InventoryItemId = 11, QuantityDonated = 70, DonationDate = DateTime.Now.AddDays(-14), DonorName = "Jessica Brown", Notes = "Merged into existing stock. Previous qty: 80" },
                new DonationLog { Id = 15, InventoryItemId = 11, QuantityDonated = 50, DonationDate = DateTime.Now.AddDays(-3), DonorName = "Community Health Center", Notes = "Merged into existing stock. Previous qty: 150" },

                // --- Id=12: Pampers Size 2 Diapers (180 units, 1 donor) ---
                new DonationLog { Id = 16, InventoryItemId = 12, QuantityDonated = 180, DonationDate = DateTime.Now.AddDays(-20), DonorName = "Daniel Garcia",  Notes = "Initial donation — item created." },

                // --- Id=13: Huggies Size 3 Diapers (165 units, 1 donor) ---
                new DonationLog { Id = 17, InventoryItemId = 13, QuantityDonated = 165, DonationDate = DateTime.Now.AddDays(-15), DonorName = "Matthew Rodriguez", Notes = "Initial donation — item created." },

                // --- Id=14: Pampers Size 4 Diapers (140 units, 1 donor) ---
                new DonationLog { Id = 18, InventoryItemId = 14, QuantityDonated = 140, DonationDate = DateTime.Now.AddDays(-10), DonorName = "Ashley Lewis",  Notes = "Initial donation — item created." },

                // --- Id=15: Huggies Size 5 Diapers (120 units, 1 donor) ---
                new DonationLog { Id = 19, InventoryItemId = 15, QuantityDonated = 120, DonationDate = DateTime.Now.AddDays(-8), DonorName = "Hannah Campbell", Notes = "Initial donation — item created." },

                // --- Id=16: Pampers Size 6 Diapers (95 units, 1 donor) ---
                new DonationLog { Id = 20, InventoryItemId = 16, QuantityDonated = 95, DonationDate = DateTime.Now.AddDays(-5), DonorName = "Nathan Parker", Notes = "Initial donation — item created." },

                // --- Id=17: Enfamil NeuroPro Formula (145 units, 2 donors) ---
                // 100 + 45 = 145
                new DonationLog { Id = 21, InventoryItemId = 17, QuantityDonated = 100, DonationDate = DateTime.Now.AddDays(-28), DonorName = "Alexis Evans",  Notes = "Initial donation — item created." },
                new DonationLog { Id = 22, InventoryItemId = 17, QuantityDonated = 45, DonationDate = DateTime.Now.AddDays(-10), DonorName = "Samuel Edwards",  Notes = "Merged into existing stock. Previous qty: 100" },

                // --- Id=18: Huggies Natural Care Wipes (220 units, 3 donors) ---
                // 100 + 80 + 40 = 220
                new DonationLog { Id = 23, InventoryItemId = 18, QuantityDonated = 100, DonationDate = DateTime.Now.AddDays(-22), DonorName = "Joshua Walker", Notes = "Initial donation — item created." },
                new DonationLog { Id = 24, InventoryItemId = 18, QuantityDonated = 80, DonationDate = DateTime.Now.AddDays(-12), DonorName = "Grace Collins", Notes = "Merged into existing stock. Previous qty: 100" },
                new DonationLog { Id = 25, InventoryItemId = 18, QuantityDonated = 40, DonationDate = DateTime.Now.AddDays(-4), DonorName = "Benjamin Stewart",  Notes = "Merged into existing stock. Previous qty: 180" },

                // --- Id=19: Diaper Rash Cream (85 units, 1 donor) ---
                new DonationLog { Id = 26, InventoryItemId = 19, QuantityDonated = 85, DonationDate = DateTime.Now.AddDays(-14), DonorName = "Amanda Hall",  Notes = "Initial donation — item created." },

                // --- Id=20: Baby Shampoo & Body Wash (135 units, 1 donor) ---
                new DonationLog { Id = 27, InventoryItemId = 20, QuantityDonated = 135, DonationDate = DateTime.Now.AddDays(-17), DonorName = "Community Health Center", Notes = "Initial donation — item created." },

                // --- Id=21: Baby Bottles 8oz (70 units, 1 donor) ---
                new DonationLog { Id = 28, InventoryItemId = 21, QuantityDonated = 70, DonationDate = DateTime.Now.AddDays(-11), DonorName = "Ryan Allen", Notes = "Initial donation — item created." },

                // --- Id=22: Bottle Nipples - Slow Flow (125 units, 1 donor) ---
                new DonationLog { Id = 29, InventoryItemId = 22, QuantityDonated = 125, DonationDate = DateTime.Now.AddDays(-16), DonorName = "Samantha Young", Notes = "Initial donation — item created." },

                // --- Id=23: Burp Cloths (90 units, 1 donor) ---
                new DonationLog { Id = 30, InventoryItemId = 23, QuantityDonated = 90, DonationDate = DateTime.Now.AddDays(-9), DonorName = "Brandon King",  Notes = "Initial donation — item created." },

                // --- Id=24: Onesies 0-3 Months (160 units, 2 donors) ---
                // 100 + 60 = 160
                new DonationLog { Id = 31, InventoryItemId = 24, QuantityDonated = 100, DonationDate = DateTime.Now.AddDays(-21), DonorName = "Nicole Wright", Notes = "Initial donation — item created." },
                new DonationLog { Id = 32, InventoryItemId = 24, QuantityDonated = 60, DonationDate = DateTime.Now.AddDays(-9), DonorName = "Justin Scott",  Notes = "Merged into existing stock. Previous qty: 100" },

                // --- Id=25: Baby Sleepers 3-6 Months (105 units, 1 donor) ---
                new DonationLog { Id = 33, InventoryItemId = 25, QuantityDonated = 105, DonationDate = DateTime.Now.AddDays(-13), DonorName = "Megan Green",  Notes = "Initial donation — item created." },

                // --- Id=26: Baby Socks (200 units, 1 donor) ---
                new DonationLog { Id = 34, InventoryItemId = 26, QuantityDonated = 200, DonationDate = DateTime.Now.AddDays(-6), DonorName = "Tyler Adams",  Notes = "Initial donation — item created." },

                // --- Id=27: Baby Mittens (80 units, 1 donor) ---
                new DonationLog { Id = 35, InventoryItemId = 27, QuantityDonated = 80, DonationDate = DateTime.Now.AddDays(-24), DonorName = "Rachel Baker", Notes = "Initial donation — item created." },

                // --- Id=28: Disposable Changing Pads (65 units, 1 donor) ---
                new DonationLog { Id = 36, InventoryItemId = 28, QuantityDonated = 65, DonationDate = DateTime.Now.AddDays(-7), DonorName = "Kevin Nelson", Notes = "Initial donation — item created." },

                // --- Id=29: Baby Nail Clippers (55 units, 1 donor) ---
                new DonationLog { Id = 37, InventoryItemId = 29, QuantityDonated = 55, DonationDate = DateTime.Now.AddDays(-4), DonorName = "Lauren Carter",  Notes = "Initial donation — item created." },

                // --- Id=30: Nasal Aspirator (45 units, 1 donor) ---
                new DonationLog { Id = 38, InventoryItemId = 30, QuantityDonated = 45, DonationDate = DateTime.Now.AddDays(-27), DonorName = "Jacob Mitchell", Notes = "Initial donation — item created." },

                // --- Id=31: Swaddle Blankets (100 units, 1 donor) ---
                new DonationLog { Id = 39, InventoryItemId = 31, QuantityDonated = 100, DonationDate = DateTime.Now.AddDays(-26), DonorName = "Kimberly Perez", Notes = "Initial donation — item created." },

                // --- Id=32: Crib Sheets (75 units, 1 donor) ---
                new DonationLog { Id = 40, InventoryItemId = 32, QuantityDonated = 75, DonationDate = DateTime.Now.AddDays(-15), DonorName = "Austin Roberts", Notes = "Initial donation — item created." },

                // --- Id=33: Baby Hooded Towels (88 units, 1 donor) ---
                new DonationLog { Id = 41, InventoryItemId = 33, QuantityDonated = 88, DonationDate = DateTime.Now.AddDays(-12), DonorName = "Brittany Turner",  Notes = "Initial donation — item created." },

                // --- Id=34: Baby Washcloths (150 units, 1 donor) ---
                new DonationLog { Id = 42, InventoryItemId = 34, QuantityDonated = 150, DonationDate = DateTime.Now.AddDays(-29), DonorName = "Costco Wholesale", Notes = "Initial donation — item created." },

                // --- Id=35: Diaper Bags (35 units, 1 donor) ---
                new DonationLog { Id = 43, InventoryItemId = 35, QuantityDonated = 35, DonationDate = DateTime.Now.AddDays(-31), DonorName = "Zachary Phillips", Notes = "Initial donation — item created." },

                // --- Id=36: Baby Thermometer (50 units, 1 donor) ---
                new DonationLog { Id = 44, InventoryItemId = 36, QuantityDonated = 50, DonationDate = DateTime.Now.AddDays(-33), DonorName = "Victoria Sanchez",  Notes = "Initial donation — item created." },

                // --- Id=37: Seventh Generation Size 1 Diapers (75 units, 1 donor) ---
                new DonationLog { Id = 45, InventoryItemId = 37, QuantityDonated = 75, DonationDate = DateTime.Now.AddDays(-12), DonorName = "Alexander Morris",  Notes = "Initial donation — item created." },

                // --- Id=38: Baby Powder (110 units, 1 donor) ---
                new DonationLog { Id = 46, InventoryItemId = 38, QuantityDonated = 110, DonationDate = DateTime.Now.AddDays(-19), DonorName = "Sophia Rogers", Notes = "Initial donation — item created." },

                // --- Id=39: Baby Lotion (115 units, 1 donor) ---
                new DonationLog { Id = 47, InventoryItemId = 39, QuantityDonated = 115, DonationDate = DateTime.Now.AddDays(-23), DonorName = "Elijah Reed", Notes = "Initial donation — item created." },

                // --- Id=40: Baby Pacifiers (140 units, 1 donor) ---
                new DonationLog { Id = 48, InventoryItemId = 40, QuantityDonated = 140, DonationDate = DateTime.Now.AddDays(-8), DonorName = "Olivia Cook",  Notes = "Initial donation — item created." },

                // --- Id=41: Baby Bibs (92 units, 1 donor) ---
                new DonationLog { Id = 49, InventoryItemId = 41, QuantityDonated = 92, DonationDate = DateTime.Now.AddDays(-42), DonorName = "Mason Morgan", Notes = "Initial donation — item created." },

                // --- Id=42: Baby Food - Sweet Potatoes (185 units, 2 donors) ---
                // 100 + 85 = 185
                new DonationLog { Id = 50, InventoryItemId = 42, QuantityDonated = 100, DonationDate = DateTime.Now.AddDays(-55), DonorName = "Ava Bell",  Notes = "Initial donation — item created." },
                new DonationLog { Id = 51, InventoryItemId = 42, QuantityDonated = 85, DonationDate = DateTime.Now.AddDays(-22), DonorName = "Lucas Murphy",Notes = "Merged into existing stock. Previous qty: 100" },

                // --- Id=43: Baby Food - Carrots (167 units, 1 donor) ---
                new DonationLog { Id = 52, InventoryItemId = 43, QuantityDonated = 167, DonationDate = DateTime.Now.AddDays(-48), DonorName = "Isabella Bailey", Notes = "Initial donation — item created." },

                // --- Id=44: Baby Spoons (78 units, 1 donor) ---
                new DonationLog { Id = 53, InventoryItemId = 44, QuantityDonated = 78, DonationDate = DateTime.Now.AddDays(-38), DonorName = "Ethan Rivera", Notes = "Initial donation — item created." },

                // --- Id=45: Baby Bowls (61 units, 1 donor) ---
                new DonationLog { Id = 54, InventoryItemId = 45, QuantityDonated = 61, DonationDate = DateTime.Now.AddDays(-52), DonorName = "Mia Cooper", Notes = "Initial donation — item created." },

                // --- Id=46: Teething Toys (103 units, 1 donor) ---
                new DonationLog { Id = 55, InventoryItemId = 46, QuantityDonated = 103, DonationDate = DateTime.Now.AddDays(-44), DonorName = "Charlotte Cox",  Notes = "Initial donation — item created." },

                // --- Id=47: Baby Sunscreen (47 units, 1 donor) ---
                new DonationLog { Id = 56, InventoryItemId = 47, QuantityDonated = 47, DonationDate = DateTime.Now.AddDays(-65), DonorName = "Aiden Howard", Notes = "Initial donation — item created." },

                // --- Id=48: Baby Laundry Detergent (82 units, 1 donor) ---
                new DonationLog { Id = 57, InventoryItemId = 48, QuantityDonated = 82, DonationDate = DateTime.Now.AddDays(-71), DonorName = "Amelia Ward",  Notes = "Initial donation — item created." },

                // --- Id=49: Baby Safety Gates (18 units, 1 donor) ---
                new DonationLog { Id = 58, InventoryItemId = 49, QuantityDonated = 18, DonationDate = DateTime.Now.AddDays(-88), DonorName = "Liam Torres",  Notes = "Initial donation — item created." },

                // --- Id=50: Outlet Covers (234 units, 3 donors) ---
                // 100 + 84 + 50 = 234
                new DonationLog { Id = 59, InventoryItemId = 50, QuantityDonated = 100, DonationDate = DateTime.Now.AddDays(-77), DonorName = "Harper Peterson", Notes = "Initial donation — item created." },
                new DonationLog { Id = 60, InventoryItemId = 50, QuantityDonated = 84, DonationDate = DateTime.Now.AddDays(-40), DonorName = "Noah Gray",Notes = "Merged into existing stock. Previous qty: 100" },
                new DonationLog { Id = 61, InventoryItemId = 50, QuantityDonated = 50, DonationDate = DateTime.Now.AddDays(-10), DonorName = "Costco Wholesale", Notes = "Merged into existing stock. Previous qty: 184" },

                // --- Id=51: Baby Monitor (12 units, 1 donor) ---
                new DonationLog { Id = 62, InventoryItemId = 51, QuantityDonated = 12, DonationDate = DateTime.Now.AddDays(-95), DonorName = "Patricia Moore", Notes = "Initial donation — item created." },

                // --- Id=52: Baby Humidifier (28 units, 1 donor) ---
                new DonationLog { Id = 63, InventoryItemId = 52, QuantityDonated = 28, DonationDate = DateTime.Now.AddDays(-102), DonorName = "Ryan Allen", Notes = "Initial donation — item created." },

                // --- Id=53: Nursing Pads (144 units, 1 donor) ---
                new DonationLog { Id = 64, InventoryItemId = 53, QuantityDonated = 144, DonationDate = DateTime.Now.AddDays(-58), DonorName = "Lisa Anderson", Notes = "Initial donation — item created." },

                // --- Id=54: Baby Bath Tub (22 units, 2 donors) ---
                // 12 + 10 = 22  ← This is the item shown in your screenshot
                new DonationLog { Id = 65, InventoryItemId = 54, QuantityDonated = 12, DonationDate = DateTime.Now.AddDays(-83), DonorName = "Logan Richardson", Notes = "Initial donation — item created." },
                new DonationLog { Id = 66, InventoryItemId = 54, QuantityDonated = 10, DonationDate = DateTime.Now.AddDays(-31), DonorName = "James Wilson",  Notes = "Merged into existing stock. Previous qty: 12" },

                // --- Id=55: Baby Hair Brush Set (67 units, 1 donor) ---
                new DonationLog { Id = 67, InventoryItemId = 55, QuantityDonated = 67, DonationDate = DateTime.Now.AddDays(-69), DonorName = "Samantha Young", Notes = "Initial donation — item created." },

                // --- Id=56: Diaper Pail Refills (98 units, 1 donor) ---
                new DonationLog { Id = 68, InventoryItemId = 56, QuantityDonated = 98, DonationDate = DateTime.Now.AddDays(-46), DonorName = "Brandon King", Notes = "Initial donation — item created." },

                // --- Id=57: Baby Carrier (15 units, 1 donor) ---
                new DonationLog { Id = 69, InventoryItemId = 57, QuantityDonated = 15, DonationDate = DateTime.Now.AddDays(-110), DonorName = "Emily Davis",  Notes = "Initial donation — item created." },

                // --- Id=58: Stroller Rain Cover (31 units, 1 donor) ---
                new DonationLog { Id = 70, InventoryItemId = 58, QuantityDonated = 31, DonationDate = DateTime.Now.AddDays(-98), DonorName = "David Thompson",  Notes = "Initial donation — item created." },

                // --- Id=59: Baby Hangers (176 units, 1 donor) ---
                new DonationLog { Id = 71, InventoryItemId = 59, QuantityDonated = 176, DonationDate = DateTime.Now.AddDays(-62), DonorName = "Target Store #1234", Notes = "Initial donation — item created." },

                // --- Id=60: Teething Gel (54 units, 1 donor) ---
                new DonationLog { Id = 72, InventoryItemId = 60, QuantityDonated = 54, DonationDate = DateTime.Now.AddDays(-75), DonorName = "Noah Gray", Notes = "Initial donation — item created." }
            );

            // ============================================================
            // ADJUSTMENT LOG SEED DATA (unchanged)
            // ============================================================
            modelBuilder.Entity<InventoryAdjustmentLog>().HasData(
                new InventoryAdjustmentLog
                {
                    Id = 1,
                    InventoryItemId = 10,
                    OldQuantity = 120,
                    NewQuantity = 150,
                    Reason = "New shipment received",
                    AdjustmentDate = DateTime.Now.AddDays(-5),
                    AdjustedBy = "Admin"
                },
                new InventoryAdjustmentLog
                {
                    Id = 2,
                    InventoryItemId = 10,
                    OldQuantity = 150,
                    NewQuantity = 130,
                    Reason = "Distributed to families",
                    AdjustmentDate = DateTime.Now.AddDays(-2),
                    AdjustedBy = "Volunteer"
                },
                new InventoryAdjustmentLog
                {
                    Id = 3,
                    InventoryItemId = 18,
                    OldQuantity = 250,
                    NewQuantity = 220,
                    Reason = "Used for community event",
                    AdjustmentDate = DateTime.Now.AddDays(-3),
                    AdjustedBy = "Admin"
                }
            );
        }
    }
}
