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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Suppress the warning so we can use DateTime.Now in seed data
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<InventoryAdjustmentLog>()
                .HasOne<InventoryItem>()
                .WithMany()
                .HasForeignKey(l => l.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comprehensive seed data for testing all features
            modelBuilder.Entity<InventoryItem>().HasData(
                // ============================================
                // EXPIRED ITEMS (Test "Expired" filter)
                // ============================================
                new InventoryItem
                {
                    Id = 1,
                    Name = "Expired Baby Formula",
                    Description = "EXPIRED - Similac Advance, 12.4 oz",
                    Quantity = 15,
                    Barcode = "070074680002",
                    ExpirationDate = DateTime.Now.AddDays(-30), // Expired 30 days ago
                    DateAdded = DateTime.Now.AddDays(-120),
                    Category = "Baby Formula",
                    DonorName = "Sarah Johnson",
                    DonorPhone = "555-0101",
                    DonorEmail = "sarah.j@email.com"
                },
                new InventoryItem
                {
                    Id = 2,
                    Name = "Expired Baby Wipes",
                    Description = "EXPIRED - Huggies Natural Care, 72 count",
                    Quantity = 8,
                    Barcode = "036000516517",
                    ExpirationDate = DateTime.Now.AddDays(-15), // Expired 15 days ago
                    DateAdded = DateTime.Now.AddDays(-90),
                    Category = "Baby Wipes",
                    DonorName = "Michael Chen",
                    DonorPhone = "555-0102",
                    DonorEmail = "m.chen@email.com"
                },
                new InventoryItem
                {
                    Id = 3,
                    Name = "Expired Diaper Cream",
                    Description = "EXPIRED - Boudreaux's Butt Paste, 4 oz",
                    Quantity = 3,
                    Barcode = "085898800015",
                    ExpirationDate = DateTime.Now.AddDays(-60), // Expired 60 days ago
                    DateAdded = DateTime.Now.AddDays(-150),
                    Category = "Baby Care",
                    DonorName = "Jennifer Martinez",
                    DonorPhone = "555-0103"
                },

                // ============================================
                // EXPIRING SOON (Within 7 days - Test "Expiring Soon" filter)
                // ============================================
                new InventoryItem
                {
                    Id = 4,
                    Name = "Gerber Good Start Formula",
                    Description = "EXPIRING SOON - Gentle powder, 32 oz",
                    Quantity = 45,
                    Barcode = "050000339877",
                    ExpirationDate = DateTime.Now.AddDays(3), // Expires in 3 days
                    DateAdded = DateTime.Now.AddDays(-60),
                    Category = "Baby Formula",
                    DonorName = "Robert Williams",
                    DonorEmail = "r.williams@email.com"
                },
                new InventoryItem
                {
                    Id = 5,
                    Name = "WaterWipes Baby Wipes",
                    Description = "EXPIRING SOON - 99.9% water, 540 count",
                    Quantity = 180,
                    Barcode = "859668006010",
                    ExpirationDate = DateTime.Now.AddDays(5), // Expires in 5 days
                    DateAdded = DateTime.Now.AddDays(-40),
                    Category = "Baby Wipes",
                    DonorName = "Lisa Anderson",
                    DonorPhone = "555-0105",
                    DonorEmail = "lisa.a@email.com"
                },
                new InventoryItem
                {
                    Id = 6,
                    Name = "Organic Baby Food - Peas",
                    Description = "EXPIRING SOON - Stage 1, 4 oz jar",
                    Quantity = 22,
                    Barcode = "051000138255",
                    ExpirationDate = DateTime.Now.AddDays(7), // Expires in 7 days
                    DateAdded = DateTime.Now.AddDays(-50),
                    Category = "Baby Food",
                    DonorName = "Target Store #1234"
                },

                // ============================================
                // EXPIRING THIS MONTH (8-30 days - Test "Expiring This Month" filter)
                // ============================================
                new InventoryItem
                {
                    Id = 7,
                    Name = "Similac Pro-Advance Formula",
                    Description = "Expiring this month - 30.8 oz powder",
                    Quantity = 130,
                    Barcode = "070074682532",
                    ExpirationDate = DateTime.Now.AddDays(25), // Expires in 25 days
                    DateAdded = DateTime.Now.AddDays(-35),
                    Category = "Baby Formula",
                    DonorName = "David Thompson",
                    DonorPhone = "555-0107"
                },
                new InventoryItem
                {
                    Id = 8,
                    Name = "Pampers Sensitive Wipes",
                    Description = "Expiring this month - 504 count",
                    Quantity = 250,
                    Barcode = "037000830689",
                    ExpirationDate = DateTime.Now.AddDays(20), // Expires in 20 days
                    DateAdded = DateTime.Now.AddDays(-18),
                    Category = "Baby Wipes",
                    DonorName = "Emily Davis",
                    DonorPhone = "555-0108",
                    DonorEmail = "emily.d@email.com"
                },
                new InventoryItem
                {
                    Id = 9,
                    Name = "Baby Vitamin D Drops",
                    Description = "Expiring this month - Liquid supplement",
                    Quantity = 68,
                    Barcode = "363824009001",
                    ExpirationDate = DateTime.Now.AddDays(28), // Expires in 28 days
                    DateAdded = DateTime.Now.AddDays(-45),
                    Category = "Baby Health",
                    DonorName = "James Wilson",
                    DonorEmail = "j.wilson@email.com"
                },

                // ============================================
                // GOOD CONDITION (More than 30 days - Test "Good" filter)
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
                    Category = "Diapers",
                    DonorName = "Patricia Moore",
                    DonorPhone = "555-0110",
                    DonorEmail = "patricia.m@email.com"
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
                    Category = "Diapers",
                    DonorName = "Walmart Store #5678",
                    DonorPhone = "555-0111"
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
                    Category = "Diapers",
                    DonorName = "Christopher Taylor",
                    DonorEmail = "chris.t@email.com"
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
                    Category = "Diapers",
                    DonorName = "Jessica Brown",
                    DonorPhone = "555-0113",
                    DonorEmail = "jessica.b@email.com"
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
                    Category = "Diapers",
                    DonorName = "Daniel Garcia",
                    DonorPhone = "555-0114"
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
                    Category = "Diapers",
                    DonorName = "St. Mary's Church"
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
                    Category = "Diapers",
                    DonorName = "Matthew Rodriguez",
                    DonorPhone = "555-0116",
                    DonorEmail = "matt.r@email.com"
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
                    Category = "Baby Formula",
                    DonorName = "Ashley Lewis",
                    DonorEmail = "ashley.l@email.com"
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
                    Category = "Baby Wipes",
                    DonorName = "Joshua Walker",
                    DonorPhone = "555-0118"
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
                    Category = "Diapering",
                    DonorName = "Amanda Hall",
                    DonorPhone = "555-0119",
                    DonorEmail = "amanda.h@email.com"
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
                    Category = "Baby Care",
                    DonorName = "Community Health Center"
                },

                // ============================================
                // NO EXPIRATION DATE (Test "No Expiration" filter)
                // ============================================
                new InventoryItem
                {
                    Id = 21,
                    Name = "Baby Bottles 8oz",
                    Description = "Dr. Brown's Anti-Colic, 3-pack",
                    Quantity = 70,
                    Barcode = "072239311004",
                    DateAdded = DateTime.Now.AddDays(-11),
                    Category = "Feeding",
                    DonorName = "Ryan Allen",
                    DonorPhone = "555-0121"
                },
                new InventoryItem
                {
                    Id = 22,
                    Name = "Bottle Nipples - Slow Flow",
                    Description = "Silicone, 6-pack, 0-3 months",
                    Quantity = 125,
                    Barcode = "072239314012",
                    DateAdded = DateTime.Now.AddDays(-16),
                    Category = "Feeding",
                    DonorName = "Samantha Young",
                    DonorEmail = "samantha.y@email.com"
                },
                new InventoryItem
                {
                    Id = 23,
                    Name = "Burp Cloths",
                    Description = "100% cotton, 10-pack set",
                    Quantity = 90,
                    Barcode = "849854012345",
                    DateAdded = DateTime.Now.AddDays(-9),
                    Category = "Feeding",
                    DonorName = "Brandon King",
                    DonorPhone = "555-0123",
                    DonorEmail = "brandon.k@email.com"
                },
                new InventoryItem
                {
                    Id = 24,
                    Name = "Onesies 0-3 Months",
                    Description = "Short sleeve, 5-pack, assorted colors",
                    Quantity = 160,
                    Barcode = "078742317298",
                    DateAdded = DateTime.Now.AddDays(-21),
                    Category = "Clothing",
                    DonorName = "Nicole Wright",
                    DonorPhone = "555-0124"
                },
                new InventoryItem
                {
                    Id = 25,
                    Name = "Baby Sleepers 3-6 Months",
                    Description = "Footed sleepers, fleece, 3-pack",
                    Quantity = 105,
                    Barcode = "078742317305",
                    DateAdded = DateTime.Now.AddDays(-13),
                    Category = "Clothing",
                    DonorName = "Justin Scott",
                    DonorEmail = "justin.s@email.com"
                },
                new InventoryItem
                {
                    Id = 26,
                    Name = "Baby Socks 0-12 Months",
                    Description = "Non-slip grip, 12-pack",
                    Quantity = 200,
                    Barcode = "849854023456",
                    DateAdded = DateTime.Now.AddDays(-6),
                    Category = "Clothing",
                    DonorName = "Megan Green",
                    DonorPhone = "555-0126",
                    DonorEmail = "megan.g@email.com"
                },
                new InventoryItem
                {
                    Id = 27,
                    Name = "Baby Mittens",
                    Description = "Scratch-free mittens, 6-pack",
                    Quantity = 80,
                    Barcode = "849854034567",
                    DateAdded = DateTime.Now.AddDays(-24),
                    Category = "Clothing",
                    DonorName = "Tyler Adams",
                    DonorPhone = "555-0127"
                },
                new InventoryItem
                {
                    Id = 28,
                    Name = "Disposable Changing Pads",
                    Description = "Waterproof, 50 count pack",
                    Quantity = 65,
                    Barcode = "735363012010",
                    DateAdded = DateTime.Now.AddDays(-7),
                    Category = "Diapering",
                    DonorName = "Rachel Baker"
                },
                new InventoryItem
                {
                    Id = 29,
                    Name = "Baby Nail Clippers",
                    Description = "Safety nail care set with file",
                    Quantity = 55,
                    Barcode = "849854045678",
                    DateAdded = DateTime.Now.AddDays(-4),
                    Category = "Baby Care",
                    DonorName = "Kevin Nelson",
                    DonorPhone = "555-0129",
                    DonorEmail = "kevin.n@email.com"
                },
                new InventoryItem
                {
                    Id = 30,
                    Name = "Nasal Aspirator",
                    Description = "NoseFrida with 20 filters",
                    Quantity = 45,
                    Barcode = "853689006133",
                    DateAdded = DateTime.Now.AddDays(-27),
                    Category = "Baby Health",
                    DonorName = "Lauren Carter",
                    DonorEmail = "lauren.c@email.com"
                },
                new InventoryItem
                {
                    Id = 31,
                    Name = "Swaddle Blankets",
                    Description = "Muslin, 4-pack, 47x47 inches",
                    Quantity = 100,
                    Barcode = "849854056789",
                    DateAdded = DateTime.Now.AddDays(-26),
                    Category = "Bedding",
                    DonorName = "Jacob Mitchell",
                    DonorPhone = "555-0131"
                },
                new InventoryItem
                {
                    Id = 32,
                    Name = "Crib Sheets",
                    Description = "Fitted, 100% cotton, 2-pack",
                    Quantity = 75,
                    Barcode = "849854067890",
                    DateAdded = DateTime.Now.AddDays(-15),
                    Category = "Bedding",
                    DonorName = "Kimberly Perez",
                    DonorPhone = "555-0132",
                    DonorEmail = "kim.p@email.com"
                },
                new InventoryItem
                {
                    Id = 33,
                    Name = "Baby Hooded Towels",
                    Description = "Extra soft, 30x30 inches, 3-pack",
                    Quantity = 88,
                    Barcode = "849854078901",
                    DateAdded = DateTime.Now.AddDays(-12),
                    Category = "Bathing",
                    DonorName = "Austin Roberts"
                },
                new InventoryItem
                {
                    Id = 34,
                    Name = "Baby Washcloths",
                    Description = "Ultra-soft, 12-pack",
                    Quantity = 150,
                    Barcode = "849854089012",
                    DateAdded = DateTime.Now.AddDays(-29),
                    Category = "Bathing",
                    DonorName = "Brittany Turner",
                    DonorEmail = "brittany.t@email.com"
                },
                new InventoryItem
                {
                    Id = 35,
                    Name = "Diaper Bags",
                    Description = "Multi-pocket backpack style",
                    Quantity = 35,
                    Barcode = "849854090123",
                    DateAdded = DateTime.Now.AddDays(-31),
                    Category = "Accessories",
                    DonorName = "Costco Wholesale",
                    DonorPhone = "555-0135"
                },
                new InventoryItem
                {
                    Id = 36,
                    Name = "Baby Thermometer",
                    Description = "Digital forehead & ear thermometer",
                    Quantity = 50,
                    Barcode = "849854101234",
                    DateAdded = DateTime.Now.AddDays(-33),
                    Category = "Baby Health",
                    DonorName = "Zachary Phillips",
                    DonorPhone = "555-0136",
                    DonorEmail = "zach.p@email.com"
                },

                // ============================================
                // ADDITIONAL ITEMS FOR PAGINATION TESTING (Items 37-60)
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
                    Category = "Diapers",
                    DonorName = "Hannah Campbell"
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
                    Category = "Baby Care",
                    DonorName = "Nathan Parker",
                    DonorPhone = "555-0138"
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
                    Category = "Baby Care",
                    DonorName = "Alexis Evans",
                    DonorEmail = "alexis.e@email.com"
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
                    Category = "Comfort",
                    DonorName = "Samuel Edwards",
                    DonorPhone = "555-0140",
                    DonorEmail = "sam.e@email.com"
                },
                new InventoryItem
                {
                    Id = 41,
                    Name = "Baby Bibs - Waterproof",
                    Description = "Silicone, easy clean, 3-pack",
                    Quantity = 92,
                    Barcode = "849854112345",
                    DateAdded = DateTime.Now.AddDays(-42),
                    Category = "Feeding",
                    DonorName = "Grace Collins"
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
                    Category = "Baby Food",
                    DonorName = "Benjamin Stewart",
                    DonorPhone = "555-0142"
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
                    Category = "Baby Food",
                    DonorName = "Victoria Sanchez",
                    DonorEmail = "victoria.s@email.com"
                },
                new InventoryItem
                {
                    Id = 44,
                    Name = "Baby Spoons",
                    Description = "Soft-tip silicone, 6-pack",
                    Quantity = 78,
                    Barcode = "849854123456",
                    DateAdded = DateTime.Now.AddDays(-38),
                    Category = "Feeding",
                    DonorName = "Alexander Morris",
                    DonorPhone = "555-0144",
                    DonorEmail = "alex.m@email.com"
                },
                new InventoryItem
                {
                    Id = 45,
                    Name = "Baby Bowls with Suction",
                    Description = "BPA-free, 3-pack with lids",
                    Quantity = 61,
                    Barcode = "849854134567",
                    DateAdded = DateTime.Now.AddDays(-52),
                    Category = "Feeding",
                    DonorName = "Sophia Rogers",
                    DonorPhone = "555-0145"
                },
                new InventoryItem
                {
                    Id = 46,
                    Name = "Teething Toys",
                    Description = "BPA-free silicone, 4-pack",
                    Quantity = 103,
                    Barcode = "849854145678",
                    DateAdded = DateTime.Now.AddDays(-44),
                    Category = "Baby Health",
                    DonorName = "Elijah Reed"
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
                    Category = "Baby Care",
                    DonorName = "Olivia Cook",
                    DonorPhone = "555-0147",
                    DonorEmail = "olivia.c@email.com"
                },
                new InventoryItem
                {
                    Id = 48,
                    Name = "Baby Laundry Detergent",
                    Description = "Dreft, hypoallergenic, 50 oz",
                    Quantity = 82,
                    Barcode = "037000120261",
                    DateAdded = DateTime.Now.AddDays(-71),
                    Category = "Baby Care",
                    DonorName = "Mason Morgan",
                    DonorEmail = "mason.m@email.com"
                },
                new InventoryItem
                {
                    Id = 49,
                    Name = "Baby Safety Gates",
                    Description = "Pressure-mounted, 29-38 inches",
                    Quantity = 18,
                    Barcode = "849854156789",
                    DateAdded = DateTime.Now.AddDays(-88),
                    Category = "Safety",
                    DonorName = "Ava Bell",
                    DonorPhone = "555-0149"
                },
                new InventoryItem
                {
                    Id = 50,
                    Name = "Outlet Covers",
                    Description = "Child-proof, 36-pack",
                    Quantity = 234,
                    Barcode = "849854167890",
                    DateAdded = DateTime.Now.AddDays(-77),
                    Category = "Safety",
                    DonorName = "Lucas Murphy",
                    DonorPhone = "555-0150",
                    DonorEmail = "lucas.m@email.com"
                },
                new InventoryItem
                {
                    Id = 51,
                    Name = "Baby Monitor",
                    Description = "Video monitor with night vision",
                    Quantity = 12,
                    Barcode = "849854178901",
                    DateAdded = DateTime.Now.AddDays(-95),
                    Category = "Safety",
                    DonorName = "Isabella Bailey"
                },
                new InventoryItem
                {
                    Id = 52,
                    Name = "Baby Humidifier",
                    Description = "Cool mist, 1 gallon capacity",
                    Quantity = 28,
                    Barcode = "849854189012",
                    DateAdded = DateTime.Now.AddDays(-102),
                    Category = "Baby Health",
                    DonorName = "Ethan Rivera",
                    DonorPhone = "555-0152"
                },
                new InventoryItem
                {
                    Id = 53,
                    Name = "Nursing Pads",
                    Description = "Disposable, 60 count box",
                    Quantity = 144,
                    Barcode = "849854190123",
                    DateAdded = DateTime.Now.AddDays(-58),
                    Category = "Feeding",
                    DonorName = "Mia Cooper",
                    DonorEmail = "mia.c@email.com"
                },
                new InventoryItem
                {
                    Id = 54,
                    Name = "Baby Bath Tub",
                    Description = "Infant to toddler, with sling",
                    Quantity = 22,
                    Barcode = "849854201234",
                    DateAdded = DateTime.Now.AddDays(-83),
                    Category = "Bathing",
                    DonorName = "Logan Richardson",
                    DonorPhone = "555-0154",
                    DonorEmail = "logan.r@email.com"
                },
                new InventoryItem
                {
                    Id = 55,
                    Name = "Baby Hair Brush Set",
                    Description = "Soft bristle brush and comb",
                    Quantity = 67,
                    Barcode = "849854212345",
                    DateAdded = DateTime.Now.AddDays(-69),
                    Category = "Baby Care",
                    DonorName = "Charlotte Cox",
                    DonorPhone = "555-0155"
                },
                new InventoryItem
                {
                    Id = 56,
                    Name = "Diaper Pail Refills",
                    Description = "Deodorizing, 6-pack rings",
                    Quantity = 98,
                    Barcode = "849854223456",
                    DateAdded = DateTime.Now.AddDays(-46),
                    Category = "Diapering",
                    DonorName = "Aiden Howard"
                },
                new InventoryItem
                {
                    Id = 57,
                    Name = "Baby Carrier",
                    Description = "Ergonomic, 8-33 lbs capacity",
                    Quantity = 15,
                    Barcode = "849854234567",
                    DateAdded = DateTime.Now.AddDays(-110),
                    Category = "Accessories",
                    DonorName = "Amelia Ward",
                    DonorPhone = "555-0157",
                    DonorEmail = "amelia.w@email.com"
                },
                new InventoryItem
                {
                    Id = 58,
                    Name = "Stroller Rain Cover",
                    Description = "Universal fit, clear PVC",
                    Quantity = 31,
                    Barcode = "849854245678",
                    DateAdded = DateTime.Now.AddDays(-98),
                    Category = "Accessories",
                    DonorName = "Liam Torres",
                    DonorEmail = "liam.t@email.com"
                },
                new InventoryItem
                {
                    Id = 59,
                    Name = "Baby Hangers",
                    Description = "Velvet non-slip, 30-pack",
                    Quantity = 176,
                    Barcode = "849854256789",
                    DateAdded = DateTime.Now.AddDays(-62),
                    Category = "Accessories",
                    DonorName = "Harper Peterson",
                    DonorPhone = "555-0159"
                },
                new InventoryItem
                {
                    Id = 60,
                    Name = "Teething Gel",
                    Description = "Natural, benzocaine-free, 0.33 oz",
                    Quantity = 54,
                    Barcode = "363824012001",
                    ExpirationDate = DateTime.Now.AddDays(2), // Expiring very soon!
                    DateAdded = DateTime.Now.AddDays(-75),
                    Category = "Baby Health",
                    DonorName = "Noah Gray",
                    DonorPhone = "555-0160",
                    DonorEmail = "noah.g@email.com"
                }
            );

            // Seed some sample adjustment logs for testing
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