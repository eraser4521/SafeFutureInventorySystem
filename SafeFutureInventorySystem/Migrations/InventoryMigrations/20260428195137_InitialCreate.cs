using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SafeFutureInventorySystem.Migrations.InventoryMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LowStockThreshold = table.Column<int>(type: "int", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdjustmentLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryItemId = table.Column<int>(type: "int", nullable: false),
                    OldQuantity = table.Column<int>(type: "int", nullable: false),
                    NewQuantity = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AdjustmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdjustedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdjustmentLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdjustmentLogs_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryItemId = table.Column<int>(type: "int", nullable: false),
                    QuantityDonated = table.Column<int>(type: "int", nullable: false),
                    DonationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DonorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonationLogs_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "InventoryItems",
                columns: new[] { "Id", "Barcode", "Category", "DateAdded", "Description", "ExpirationDate", "LastUpdated", "LowStockThreshold", "Name", "Quantity" },
                values: new object[,]
                {
                    { 1, "070074680002", "Baby Formula", new DateTime(2025, 12, 29, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4271), "EXPIRED - Similac Advance, 12.4 oz", new DateTime(2026, 3, 29, 15, 51, 37, 543, DateTimeKind.Local).AddTicks(8980), null, 0, "Expired Baby Formula", 15 },
                    { 2, "036000516517", "Baby Wipes", new DateTime(2026, 1, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4636), "EXPIRED - Huggies Natural Care, 72 count", new DateTime(2026, 4, 13, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4631), null, 0, "Expired Baby Wipes", 8 },
                    { 3, "085898800015", "Baby Care", new DateTime(2025, 11, 29, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4641), "EXPIRED - Boudreaux's Butt Paste, 4 oz", new DateTime(2026, 2, 27, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4639), null, 0, "Expired Diaper Cream", 3 },
                    { 4, "050000339877", "Baby Formula", new DateTime(2026, 2, 27, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4645), "EXPIRING SOON - Gentle powder, 32 oz", new DateTime(2026, 5, 1, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4644), null, 0, "Gerber Good Start Formula", 45 },
                    { 5, "859668006010", "Baby Wipes", new DateTime(2026, 3, 19, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4666), "EXPIRING SOON - 99.9% water, 540 count", new DateTime(2026, 5, 3, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4665), null, 0, "WaterWipes Baby Wipes", 180 },
                    { 6, "051000138255", "Baby Food", new DateTime(2026, 3, 9, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4669), "EXPIRING SOON - Stage 1, 4 oz jar", new DateTime(2026, 5, 5, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4668), null, 0, "Organic Baby Food - Peas", 22 },
                    { 7, "070074682532", "Baby Formula", new DateTime(2026, 3, 24, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4672), "Expiring this month - 30.8 oz powder", new DateTime(2026, 5, 23, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4671), null, 0, "Similac Pro-Advance Formula", 130 },
                    { 8, "037000830689", "Baby Wipes", new DateTime(2026, 4, 10, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4675), "Expiring this month - 504 count", new DateTime(2026, 5, 18, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4674), null, 0, "Pampers Sensitive Wipes", 250 },
                    { 9, "363824009001", "Baby Health", new DateTime(2026, 3, 14, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4678), "Expiring this month - Liquid supplement", new DateTime(2026, 5, 26, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4677), null, 0, "Baby Vitamin D Drops", 68 },
                    { 10, "037000465911", "Diapers", new DateTime(2026, 3, 29, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4696), "Size N, Up to 10 lbs, 32 count pack", new DateTime(2027, 10, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4680), null, 0, "Pampers Newborn Diapers", 150 },
                    { 11, "036000406481", "Diapers", new DateTime(2026, 4, 3, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4700), "Size 1, 8-14 lbs, 84 count pack", new DateTime(2028, 4, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4698), null, 0, "Huggies Size 1 Diapers", 200 },
                    { 12, "037000465928", "Diapers", new DateTime(2026, 4, 8, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4703), "Size 2, 12-18 lbs, 112 count pack", new DateTime(2027, 12, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4702), null, 0, "Pampers Size 2 Diapers", 180 },
                    { 13, "036000406498", "Diapers", new DateTime(2026, 4, 13, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4716), "Size 3, 16-28 lbs, 104 count pack", new DateTime(2028, 2, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4714), null, 0, "Huggies Size 3 Diapers", 165 },
                    { 14, "037000465935", "Diapers", new DateTime(2026, 4, 18, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4719), "Size 4, 22-37 lbs, 92 count pack", new DateTime(2027, 11, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4717), null, 0, "Pampers Size 4 Diapers", 140 },
                    { 15, "036000406504", "Diapers", new DateTime(2026, 4, 20, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4729), "Size 5, 27+ lbs, 80 count pack", new DateTime(2028, 1, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4728), null, 0, "Huggies Size 5 Diapers", 120 },
                    { 16, "037000465942", "Diapers", new DateTime(2026, 4, 23, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4732), "Size 6, 35+ lbs, 68 count pack", new DateTime(2028, 3, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4731), null, 0, "Pampers Size 6 Diapers", 95 },
                    { 17, "300871214415", "Baby Formula", new DateTime(2026, 3, 31, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4736), "Infant formula powder, 28.3 oz", new DateTime(2027, 2, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4735), null, 0, "Enfamil NeuroPro Formula", 145 },
                    { 18, "036000516500", "Baby Wipes", new DateTime(2026, 4, 6, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4739), "Fragrance-free, 552 count (8 packs)", new DateTime(2027, 6, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4738), null, 0, "Huggies Natural Care Wipes", 220 },
                    { 19, "067981105001", "Diapering", new DateTime(2026, 4, 14, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4742), "Desitin Maximum Strength, 4 oz tube", new DateTime(2027, 10, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4741), null, 0, "Diaper Rash Cream", 85 },
                    { 20, "381371161423", "Baby Care", new DateTime(2026, 4, 11, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4745), "Johnson's Head-to-Toe, 27.1 fl oz", new DateTime(2028, 10, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4743), null, 0, "Baby Shampoo & Body Wash", 135 },
                    { 21, "072239311004", "Feeding", new DateTime(2026, 4, 17, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4746), "Dr. Brown's Anti-Colic, 3-pack", null, null, 0, "Baby Bottles 8oz", 70 },
                    { 22, "072239314012", "Feeding", new DateTime(2026, 4, 12, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4748), "Silicone, 6-pack, 0-3 months", null, null, 0, "Bottle Nipples - Slow Flow", 125 },
                    { 23, "849854012345", "Feeding", new DateTime(2026, 4, 19, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4750), "100% cotton, 10-pack set", null, null, 0, "Burp Cloths", 90 },
                    { 24, "078742317298", "Clothing", new DateTime(2026, 4, 7, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4751), "Short sleeve, 5-pack, assorted colors", null, null, 0, "Onesies 0-3 Months", 160 },
                    { 25, "078742317305", "Clothing", new DateTime(2026, 4, 15, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4753), "Footed sleepers, fleece, 3-pack", null, null, 0, "Baby Sleepers 3-6 Months", 105 },
                    { 26, "849854023456", "Clothing", new DateTime(2026, 4, 22, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4755), "Non-slip grip, 12-pack", null, null, 0, "Baby Socks 0-12 Months", 200 },
                    { 27, "849854034567", "Clothing", new DateTime(2026, 4, 4, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4764), "Scratch-free mittens, 6-pack", null, null, 0, "Baby Mittens", 80 },
                    { 28, "735363012010", "Diapering", new DateTime(2026, 4, 21, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4766), "Waterproof, 50 count pack", null, null, 0, "Disposable Changing Pads", 65 },
                    { 29, "849854045678", "Baby Care", new DateTime(2026, 4, 24, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4768), "Safety nail care set with file", null, null, 0, "Baby Nail Clippers", 55 },
                    { 30, "853689006133", "Baby Health", new DateTime(2026, 4, 1, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4770), "NoseFrida with 20 filters", null, null, 0, "Nasal Aspirator", 45 },
                    { 31, "849854056789", "Bedding", new DateTime(2026, 4, 2, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4772), "Muslin, 4-pack, 47x47 inches", null, null, 0, "Swaddle Blankets", 100 },
                    { 32, "849854067890", "Bedding", new DateTime(2026, 4, 13, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4774), "Fitted, 100% cotton, 2-pack", null, null, 0, "Crib Sheets", 75 },
                    { 33, "849854078901", "Bathing", new DateTime(2026, 4, 16, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4775), "Extra soft, 30x30 inches, 3-pack", null, null, 0, "Baby Hooded Towels", 88 },
                    { 34, "849854089012", "Bathing", new DateTime(2026, 3, 30, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4777), "Ultra-soft, 12-pack", null, null, 0, "Baby Washcloths", 150 },
                    { 35, "849854090123", "Accessories", new DateTime(2026, 3, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4779), "Multi-pocket backpack style", null, null, 0, "Diaper Bags", 35 },
                    { 36, "849854101234", "Baby Health", new DateTime(2026, 3, 26, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4781), "Digital forehead & ear thermometer", null, null, 0, "Baby Thermometer", 50 },
                    { 37, "732913441914", "Diapers", new DateTime(2026, 4, 16, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4784), "Eco-friendly, Size 1, 8-14 lbs, 40 count", new DateTime(2027, 7, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4783), null, 0, "Seventh Generation Size 1 Diapers", 75 },
                    { 38, "381371161416", "Baby Care", new DateTime(2026, 4, 9, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4787), "Johnson's Baby Powder, cornstarch, 9 oz", new DateTime(2028, 4, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4785), null, 0, "Baby Powder", 110 },
                    { 39, "381371161430", "Baby Care", new DateTime(2026, 4, 5, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4798), "Aveeno Baby Daily Moisture, 18 fl oz", new DateTime(2028, 6, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4788), null, 0, "Baby Lotion", 115 },
                    { 40, "072239314029", "Comfort", new DateTime(2026, 4, 20, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4801), "Orthodontic, BPA-free, 4-pack", new DateTime(2029, 4, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4800), null, 0, "Baby Pacifiers 0-6 Months", 140 },
                    { 41, "849854112345", "Feeding", new DateTime(2026, 3, 17, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4802), "Silicone, easy clean, 3-pack", null, null, 0, "Baby Bibs - Waterproof", 92 },
                    { 42, "051000138262", "Baby Food", new DateTime(2026, 3, 4, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4805), "Organic stage 1, 4 oz jar", new DateTime(2026, 6, 12, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4804), null, 0, "Baby Food - Sweet Potatoes", 185 },
                    { 43, "051000138279", "Baby Food", new DateTime(2026, 3, 11, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4808), "Organic stage 1, 4 oz jar", new DateTime(2026, 6, 17, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4807), null, 0, "Baby Food - Carrots", 167 },
                    { 44, "849854123456", "Feeding", new DateTime(2026, 3, 21, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4810), "Soft-tip silicone, 6-pack", null, null, 0, "Baby Spoons", 78 },
                    { 45, "849854134567", "Feeding", new DateTime(2026, 3, 7, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4811), "BPA-free, 3-pack with lids", null, null, 0, "Baby Bowls with Suction", 61 },
                    { 46, "849854145678", "Baby Health", new DateTime(2026, 3, 15, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4813), "BPA-free silicone, 4-pack", null, null, 0, "Teething Toys", 103 },
                    { 47, "738443002151", "Baby Care", new DateTime(2026, 2, 22, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4816), "Mineral-based, tear-free, 3 oz", new DateTime(2027, 8, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4815), null, 0, "Baby Sunscreen SPF 50", 47 },
                    { 48, "037000120261", "Baby Care", new DateTime(2026, 2, 16, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4818), "Dreft, hypoallergenic, 50 oz", null, null, 0, "Baby Laundry Detergent", 82 },
                    { 49, "849854156789", "Safety", new DateTime(2026, 1, 30, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4820), "Pressure-mounted, 29-38 inches", null, null, 0, "Baby Safety Gates", 18 },
                    { 50, "849854167890", "Safety", new DateTime(2026, 2, 10, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4822), "Child-proof, 36-pack", null, null, 0, "Outlet Covers", 234 },
                    { 51, "849854178901", "Safety", new DateTime(2026, 1, 23, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4823), "Video monitor with night vision", null, null, 0, "Baby Monitor", 12 },
                    { 52, "849854189012", "Baby Health", new DateTime(2026, 1, 16, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4832), "Cool mist, 1 gallon capacity", null, null, 0, "Baby Humidifier", 28 },
                    { 53, "849854190123", "Feeding", new DateTime(2026, 3, 1, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4834), "Disposable, 60 count box", null, null, 0, "Nursing Pads", 144 },
                    { 54, "849854201234", "Bathing", new DateTime(2026, 2, 4, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4836), "Infant to toddler, with sling", null, null, 0, "Baby Bath Tub", 22 },
                    { 55, "849854212345", "Baby Care", new DateTime(2026, 2, 18, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4838), "Soft bristle brush and comb", null, null, 0, "Baby Hair Brush Set", 67 },
                    { 56, "849854223456", "Diapering", new DateTime(2026, 3, 13, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4840), "Deodorizing, 6-pack rings", null, null, 0, "Diaper Pail Refills", 98 },
                    { 57, "849854234567", "Accessories", new DateTime(2026, 1, 8, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4850), "Ergonomic, 8-33 lbs capacity", null, null, 0, "Baby Carrier", 15 },
                    { 58, "849854245678", "Accessories", new DateTime(2026, 1, 20, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4852), "Universal fit, clear PVC", null, null, 0, "Stroller Rain Cover", 31 },
                    { 59, "849854256789", "Accessories", new DateTime(2026, 2, 25, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4854), "Velvet non-slip, 30-pack", null, null, 0, "Baby Hangers", 176 },
                    { 60, "363824012001", "Baby Health", new DateTime(2026, 2, 12, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4857), "Natural, benzocaine-free, 0.33 oz", new DateTime(2026, 4, 30, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(4856), null, 0, "Teething Gel", 54 }
                });

            migrationBuilder.InsertData(
                table: "AdjustmentLogs",
                columns: new[] { "Id", "AdjustedBy", "AdjustmentDate", "InventoryItemId", "NewQuantity", "OldQuantity", "Reason" },
                values: new object[,]
                {
                    { 1, "Admin", new DateTime(2026, 4, 23, 15, 51, 37, 546, DateTimeKind.Local).AddTicks(132), 10, 150, 120, "New shipment received" },
                    { 2, "Volunteer", new DateTime(2026, 4, 26, 15, 51, 37, 546, DateTimeKind.Local).AddTicks(301), 10, 130, 150, "Distributed to families" },
                    { 3, "Admin", new DateTime(2026, 4, 25, 15, 51, 37, 546, DateTimeKind.Local).AddTicks(304), 18, 220, 250, "Used for community event" }
                });

            migrationBuilder.InsertData(
                table: "DonationLogs",
                columns: new[] { "Id", "DonationDate", "DonorName", "InventoryItemId", "Notes", "QuantityDonated" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 29, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(8997), "Sarah Johnson", 1, "Initial donation — item created.", 15 },
                    { 2, new DateTime(2026, 1, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9309), "Michael Chen", 2, "Initial donation — item created.", 8 },
                    { 3, new DateTime(2025, 11, 29, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9313), "Jennifer Martinez", 3, "Initial donation — item created.", 3 },
                    { 4, new DateTime(2026, 2, 27, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9315), "Robert Williams", 4, "Initial donation — item created.", 45 },
                    { 5, new DateTime(2026, 3, 19, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9318), "Lisa Anderson", 5, "Initial donation — item created.", 180 },
                    { 6, new DateTime(2026, 3, 9, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9330), "Target Store #1234", 6, "Initial donation — item created.", 22 },
                    { 7, new DateTime(2026, 3, 24, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9332), "David Thompson", 7, "Initial donation — item created.", 130 },
                    { 8, new DateTime(2026, 4, 10, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9334), "Emily Davis", 8, "Initial donation — item created.", 250 },
                    { 9, new DateTime(2026, 3, 14, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9339), "James Wilson", 9, "Initial donation — item created.", 68 },
                    { 10, new DateTime(2026, 3, 29, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9341), "Patricia Moore", 10, "Initial donation — item created.", 60 },
                    { 11, new DateTime(2026, 4, 10, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9343), "Walmart Store #5678", 10, "Merged into existing stock. Previous qty: 60", 50 },
                    { 12, new DateTime(2026, 4, 21, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9345), "St. Mary's Church", 10, "Merged into existing stock. Previous qty: 110", 40 },
                    { 13, new DateTime(2026, 4, 3, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9346), "Christopher Taylor", 11, "Initial donation — item created.", 80 },
                    { 14, new DateTime(2026, 4, 14, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9348), "Jessica Brown", 11, "Merged into existing stock. Previous qty: 80", 70 },
                    { 15, new DateTime(2026, 4, 25, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9350), "Community Health Center", 11, "Merged into existing stock. Previous qty: 150", 50 },
                    { 16, new DateTime(2026, 4, 8, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9352), "Daniel Garcia", 12, "Initial donation — item created.", 180 },
                    { 17, new DateTime(2026, 4, 13, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9353), "Matthew Rodriguez", 13, "Initial donation — item created.", 165 },
                    { 18, new DateTime(2026, 4, 18, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9355), "Ashley Lewis", 14, "Initial donation — item created.", 140 },
                    { 19, new DateTime(2026, 4, 20, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9357), "Hannah Campbell", 15, "Initial donation — item created.", 120 },
                    { 20, new DateTime(2026, 4, 23, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9358), "Nathan Parker", 16, "Initial donation — item created.", 95 },
                    { 21, new DateTime(2026, 3, 31, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9367), "Alexis Evans", 17, "Initial donation — item created.", 100 },
                    { 22, new DateTime(2026, 4, 18, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9368), "Samuel Edwards", 17, "Merged into existing stock. Previous qty: 100", 45 },
                    { 23, new DateTime(2026, 4, 6, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9370), "Joshua Walker", 18, "Initial donation — item created.", 100 },
                    { 24, new DateTime(2026, 4, 16, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9371), "Grace Collins", 18, "Merged into existing stock. Previous qty: 100", 80 },
                    { 25, new DateTime(2026, 4, 24, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9373), "Benjamin Stewart", 18, "Merged into existing stock. Previous qty: 180", 40 },
                    { 26, new DateTime(2026, 4, 14, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9374), "Amanda Hall", 19, "Initial donation — item created.", 85 },
                    { 27, new DateTime(2026, 4, 11, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9376), "Community Health Center", 20, "Initial donation — item created.", 135 },
                    { 28, new DateTime(2026, 4, 17, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9377), "Ryan Allen", 21, "Initial donation — item created.", 70 },
                    { 29, new DateTime(2026, 4, 12, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9379), "Samantha Young", 22, "Initial donation — item created.", 125 },
                    { 30, new DateTime(2026, 4, 19, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9381), "Brandon King", 23, "Initial donation — item created.", 90 },
                    { 31, new DateTime(2026, 4, 7, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9382), "Nicole Wright", 24, "Initial donation — item created.", 100 },
                    { 32, new DateTime(2026, 4, 19, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9384), "Justin Scott", 24, "Merged into existing stock. Previous qty: 100", 60 },
                    { 33, new DateTime(2026, 4, 15, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9385), "Megan Green", 25, "Initial donation — item created.", 105 },
                    { 34, new DateTime(2026, 4, 22, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9387), "Tyler Adams", 26, "Initial donation — item created.", 200 },
                    { 35, new DateTime(2026, 4, 4, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9388), "Rachel Baker", 27, "Initial donation — item created.", 80 },
                    { 36, new DateTime(2026, 4, 21, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9390), "Kevin Nelson", 28, "Initial donation — item created.", 65 },
                    { 37, new DateTime(2026, 4, 24, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9399), "Lauren Carter", 29, "Initial donation — item created.", 55 },
                    { 38, new DateTime(2026, 4, 1, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9401), "Jacob Mitchell", 30, "Initial donation — item created.", 45 },
                    { 39, new DateTime(2026, 4, 2, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9402), "Kimberly Perez", 31, "Initial donation — item created.", 100 },
                    { 40, new DateTime(2026, 4, 13, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9404), "Austin Roberts", 32, "Initial donation — item created.", 75 },
                    { 41, new DateTime(2026, 4, 16, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9406), "Brittany Turner", 33, "Initial donation — item created.", 88 },
                    { 42, new DateTime(2026, 3, 30, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9407), "Costco Wholesale", 34, "Initial donation — item created.", 150 },
                    { 43, new DateTime(2026, 3, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9409), "Zachary Phillips", 35, "Initial donation — item created.", 35 },
                    { 44, new DateTime(2026, 3, 26, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9410), "Victoria Sanchez", 36, "Initial donation — item created.", 50 },
                    { 45, new DateTime(2026, 4, 16, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9412), "Alexander Morris", 37, "Initial donation — item created.", 75 },
                    { 46, new DateTime(2026, 4, 9, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9413), "Sophia Rogers", 38, "Initial donation — item created.", 110 },
                    { 47, new DateTime(2026, 4, 5, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9415), "Elijah Reed", 39, "Initial donation — item created.", 115 },
                    { 48, new DateTime(2026, 4, 20, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9416), "Olivia Cook", 40, "Initial donation — item created.", 140 },
                    { 49, new DateTime(2026, 3, 17, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9418), "Mason Morgan", 41, "Initial donation — item created.", 92 },
                    { 50, new DateTime(2026, 3, 4, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9420), "Ava Bell", 42, "Initial donation — item created.", 100 },
                    { 51, new DateTime(2026, 4, 6, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9421), "Lucas Murphy", 42, "Merged into existing stock. Previous qty: 100", 85 },
                    { 52, new DateTime(2026, 3, 11, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9423), "Isabella Bailey", 43, "Initial donation — item created.", 167 },
                    { 53, new DateTime(2026, 3, 21, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9432), "Ethan Rivera", 44, "Initial donation — item created.", 78 },
                    { 54, new DateTime(2026, 3, 7, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9433), "Mia Cooper", 45, "Initial donation — item created.", 61 },
                    { 55, new DateTime(2026, 3, 15, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9435), "Charlotte Cox", 46, "Initial donation — item created.", 103 },
                    { 56, new DateTime(2026, 2, 22, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9437), "Aiden Howard", 47, "Initial donation — item created.", 47 },
                    { 57, new DateTime(2026, 2, 16, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9438), "Amelia Ward", 48, "Initial donation — item created.", 82 },
                    { 58, new DateTime(2026, 1, 30, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9440), "Liam Torres", 49, "Initial donation — item created.", 18 },
                    { 59, new DateTime(2026, 2, 10, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9441), "Harper Peterson", 50, "Initial donation — item created.", 100 },
                    { 60, new DateTime(2026, 3, 19, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9443), "Noah Gray", 50, "Merged into existing stock. Previous qty: 100", 84 },
                    { 61, new DateTime(2026, 4, 18, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9445), "Costco Wholesale", 50, "Merged into existing stock. Previous qty: 184", 50 },
                    { 62, new DateTime(2026, 1, 23, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9446), "Patricia Moore", 51, "Initial donation — item created.", 12 },
                    { 63, new DateTime(2026, 1, 16, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9448), "Ryan Allen", 52, "Initial donation — item created.", 28 },
                    { 64, new DateTime(2026, 3, 1, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9449), "Lisa Anderson", 53, "Initial donation — item created.", 144 },
                    { 65, new DateTime(2026, 2, 4, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9451), "Logan Richardson", 54, "Initial donation — item created.", 12 },
                    { 66, new DateTime(2026, 3, 28, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9452), "James Wilson", 54, "Merged into existing stock. Previous qty: 12", 10 },
                    { 67, new DateTime(2026, 2, 18, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9454), "Samantha Young", 55, "Initial donation — item created.", 67 },
                    { 68, new DateTime(2026, 3, 13, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9456), "Brandon King", 56, "Initial donation — item created.", 98 },
                    { 69, new DateTime(2026, 1, 8, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9458), "Emily Davis", 57, "Initial donation — item created.", 15 },
                    { 70, new DateTime(2026, 1, 20, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9460), "David Thompson", 58, "Initial donation — item created.", 31 },
                    { 71, new DateTime(2026, 2, 25, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9461), "Target Store #1234", 59, "Initial donation — item created.", 176 },
                    { 72, new DateTime(2026, 2, 12, 15, 51, 37, 545, DateTimeKind.Local).AddTicks(9463), "Noah Gray", 60, "Initial donation — item created.", 54 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdjustmentLogs_InventoryItemId",
                table: "AdjustmentLogs",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DonationLogs_InventoryItemId",
                table: "DonationLogs",
                column: "InventoryItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdjustmentLogs");

            migrationBuilder.DropTable(
                name: "DonationLogs");

            migrationBuilder.DropTable(
                name: "InventoryItems");
        }
    }
}
