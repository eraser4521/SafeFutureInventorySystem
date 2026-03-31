using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SafeFutureInventorySystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QrCodes");

            migrationBuilder.DropColumn(
                name: "DonorEmail",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "DonorName",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "DonorPhone",
                table: "InventoryItems");

            migrationBuilder.CreateTable(
                name: "DonationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InventoryItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityDonated = table.Column<int>(type: "INTEGER", nullable: false),
                    DonationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DonorName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
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

            migrationBuilder.UpdateData(
                table: "AdjustmentLogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdjustmentDate",
                value: new DateTime(2026, 3, 26, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(3900));

            migrationBuilder.UpdateData(
                table: "AdjustmentLogs",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdjustmentDate",
                value: new DateTime(2026, 3, 29, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(4300));

            migrationBuilder.UpdateData(
                table: "AdjustmentLogs",
                keyColumn: "Id",
                keyValue: 3,
                column: "AdjustmentDate",
                value: new DateTime(2026, 3, 28, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(4300));

            migrationBuilder.InsertData(
                table: "DonationLogs",
                columns: new[] { "Id", "DonationDate", "DonorName", "InventoryItemId", "Notes", "QuantityDonated" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 1, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(1290), "Sarah Johnson", 1, "Initial donation — item created.", 15 },
                    { 2, new DateTime(2025, 12, 31, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(1930), "Michael Chen", 2, "Initial donation — item created.", 8 },
                    { 3, new DateTime(2025, 11, 1, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(1940), "Jennifer Martinez", 3, "Initial donation — item created.", 3 },
                    { 4, new DateTime(2026, 1, 30, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2050), "Robert Williams", 4, "Initial donation — item created.", 45 },
                    { 5, new DateTime(2026, 2, 19, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2060), "Lisa Anderson", 5, "Initial donation — item created.", 180 },
                    { 6, new DateTime(2026, 2, 9, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2060), "Target Store #1234", 6, "Initial donation — item created.", 22 },
                    { 7, new DateTime(2026, 2, 24, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2070), "David Thompson", 7, "Initial donation — item created.", 130 },
                    { 8, new DateTime(2026, 3, 13, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2070), "Emily Davis", 8, "Initial donation — item created.", 250 },
                    { 9, new DateTime(2026, 2, 14, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2070), "James Wilson", 9, "Initial donation — item created.", 68 },
                    { 10, new DateTime(2026, 3, 1, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2080), "Patricia Moore", 10, "Initial donation — item created.", 60 },
                    { 11, new DateTime(2026, 3, 13, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2090), "Walmart Store #5678", 10, "Merged into existing stock. Previous qty: 60", 50 },
                    { 12, new DateTime(2026, 3, 24, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2090), "St. Mary's Church", 10, "Merged into existing stock. Previous qty: 110", 40 },
                    { 13, new DateTime(2026, 3, 6, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2100), "Christopher Taylor", 11, "Initial donation — item created.", 80 },
                    { 14, new DateTime(2026, 3, 17, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2100), "Jessica Brown", 11, "Merged into existing stock. Previous qty: 80", 70 },
                    { 15, new DateTime(2026, 3, 28, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2100), "Community Health Center", 11, "Merged into existing stock. Previous qty: 150", 50 },
                    { 16, new DateTime(2026, 3, 11, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2110), "Daniel Garcia", 12, "Initial donation — item created.", 180 },
                    { 17, new DateTime(2026, 3, 16, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2110), "Matthew Rodriguez", 13, "Initial donation — item created.", 165 },
                    { 18, new DateTime(2026, 3, 21, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2120), "Ashley Lewis", 14, "Initial donation — item created.", 140 },
                    { 19, new DateTime(2026, 3, 23, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2120), "Hannah Campbell", 15, "Initial donation — item created.", 120 },
                    { 20, new DateTime(2026, 3, 26, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2130), "Nathan Parker", 16, "Initial donation — item created.", 95 },
                    { 21, new DateTime(2026, 3, 3, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2140), "Alexis Evans", 17, "Initial donation — item created.", 100 },
                    { 22, new DateTime(2026, 3, 21, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2140), "Samuel Edwards", 17, "Merged into existing stock. Previous qty: 100", 45 },
                    { 23, new DateTime(2026, 3, 9, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2150), "Joshua Walker", 18, "Initial donation — item created.", 100 },
                    { 24, new DateTime(2026, 3, 19, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2150), "Grace Collins", 18, "Merged into existing stock. Previous qty: 100", 80 },
                    { 25, new DateTime(2026, 3, 27, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2150), "Benjamin Stewart", 18, "Merged into existing stock. Previous qty: 180", 40 },
                    { 26, new DateTime(2026, 3, 17, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2160), "Amanda Hall", 19, "Initial donation — item created.", 85 },
                    { 27, new DateTime(2026, 3, 14, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2160), "Community Health Center", 20, "Initial donation — item created.", 135 },
                    { 28, new DateTime(2026, 3, 20, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2170), "Ryan Allen", 21, "Initial donation — item created.", 70 },
                    { 29, new DateTime(2026, 3, 15, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2170), "Samantha Young", 22, "Initial donation — item created.", 125 },
                    { 30, new DateTime(2026, 3, 22, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2180), "Brandon King", 23, "Initial donation — item created.", 90 },
                    { 31, new DateTime(2026, 3, 10, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2180), "Nicole Wright", 24, "Initial donation — item created.", 100 },
                    { 32, new DateTime(2026, 3, 22, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2190), "Justin Scott", 24, "Merged into existing stock. Previous qty: 100", 60 },
                    { 33, new DateTime(2026, 3, 18, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2190), "Megan Green", 25, "Initial donation — item created.", 105 },
                    { 34, new DateTime(2026, 3, 25, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2200), "Tyler Adams", 26, "Initial donation — item created.", 200 },
                    { 35, new DateTime(2026, 3, 7, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2200), "Rachel Baker", 27, "Initial donation — item created.", 80 },
                    { 36, new DateTime(2026, 3, 24, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2210), "Kevin Nelson", 28, "Initial donation — item created.", 65 },
                    { 37, new DateTime(2026, 3, 27, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2210), "Lauren Carter", 29, "Initial donation — item created.", 55 },
                    { 38, new DateTime(2026, 3, 4, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2220), "Jacob Mitchell", 30, "Initial donation — item created.", 45 },
                    { 39, new DateTime(2026, 3, 5, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2220), "Kimberly Perez", 31, "Initial donation — item created.", 100 },
                    { 40, new DateTime(2026, 3, 16, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2230), "Austin Roberts", 32, "Initial donation — item created.", 75 },
                    { 41, new DateTime(2026, 3, 19, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2230), "Brittany Turner", 33, "Initial donation — item created.", 88 },
                    { 42, new DateTime(2026, 3, 2, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2230), "Costco Wholesale", 34, "Initial donation — item created.", 150 },
                    { 43, new DateTime(2026, 2, 28, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2240), "Zachary Phillips", 35, "Initial donation — item created.", 35 },
                    { 44, new DateTime(2026, 2, 26, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2240), "Victoria Sanchez", 36, "Initial donation — item created.", 50 },
                    { 45, new DateTime(2026, 3, 19, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2250), "Alexander Morris", 37, "Initial donation — item created.", 75 },
                    { 46, new DateTime(2026, 3, 12, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2260), "Sophia Rogers", 38, "Initial donation — item created.", 110 },
                    { 47, new DateTime(2026, 3, 8, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2260), "Elijah Reed", 39, "Initial donation — item created.", 115 },
                    { 48, new DateTime(2026, 3, 23, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2270), "Olivia Cook", 40, "Initial donation — item created.", 140 },
                    { 49, new DateTime(2026, 2, 17, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2270), "Mason Morgan", 41, "Initial donation — item created.", 92 },
                    { 50, new DateTime(2026, 2, 4, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2270), "Ava Bell", 42, "Initial donation — item created.", 100 },
                    { 51, new DateTime(2026, 3, 9, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2280), "Lucas Murphy", 42, "Merged into existing stock. Previous qty: 100", 85 },
                    { 52, new DateTime(2026, 2, 11, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2280), "Isabella Bailey", 43, "Initial donation — item created.", 167 },
                    { 53, new DateTime(2026, 2, 21, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2290), "Ethan Rivera", 44, "Initial donation — item created.", 78 },
                    { 54, new DateTime(2026, 2, 7, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2300), "Mia Cooper", 45, "Initial donation — item created.", 61 },
                    { 55, new DateTime(2026, 2, 15, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2300), "Charlotte Cox", 46, "Initial donation — item created.", 103 },
                    { 56, new DateTime(2026, 1, 25, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2310), "Aiden Howard", 47, "Initial donation — item created.", 47 },
                    { 57, new DateTime(2026, 1, 19, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2310), "Amelia Ward", 48, "Initial donation — item created.", 82 },
                    { 58, new DateTime(2026, 1, 2, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2310), "Liam Torres", 49, "Initial donation — item created.", 18 },
                    { 59, new DateTime(2026, 1, 13, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2320), "Harper Peterson", 50, "Initial donation — item created.", 100 },
                    { 60, new DateTime(2026, 2, 19, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2320), "Noah Gray", 50, "Merged into existing stock. Previous qty: 100", 84 },
                    { 61, new DateTime(2026, 3, 21, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2330), "Costco Wholesale", 50, "Merged into existing stock. Previous qty: 184", 50 },
                    { 62, new DateTime(2025, 12, 26, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2330), "Patricia Moore", 51, "Initial donation — item created.", 12 },
                    { 63, new DateTime(2025, 12, 19, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2340), "Ryan Allen", 52, "Initial donation — item created.", 28 },
                    { 64, new DateTime(2026, 2, 1, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2340), "Lisa Anderson", 53, "Initial donation — item created.", 144 },
                    { 65, new DateTime(2026, 1, 7, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2350), "Logan Richardson", 54, "Initial donation — item created.", 12 },
                    { 66, new DateTime(2026, 2, 28, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2350), "James Wilson", 54, "Merged into existing stock. Previous qty: 12", 10 },
                    { 67, new DateTime(2026, 1, 21, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2350), "Samantha Young", 55, "Initial donation — item created.", 67 },
                    { 68, new DateTime(2026, 2, 13, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2360), "Brandon King", 56, "Initial donation — item created.", 98 },
                    { 69, new DateTime(2025, 12, 11, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2370), "Emily Davis", 57, "Initial donation — item created.", 15 },
                    { 70, new DateTime(2025, 12, 23, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2370), "David Thompson", 58, "Initial donation — item created.", 31 },
                    { 71, new DateTime(2026, 1, 28, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2380), "Target Store #1234", 59, "Initial donation — item created.", 176 },
                    { 72, new DateTime(2026, 1, 15, 5, 41, 52, 379, DateTimeKind.Local).AddTicks(2380), "Noah Gray", 60, "Initial donation — item created.", 54 }
                });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 1, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4240), new DateTime(2026, 3, 1, 5, 41, 52, 368, DateTimeKind.Local).AddTicks(8600) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4770), new DateTime(2026, 3, 16, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4760) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 11, 1, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4780), new DateTime(2026, 1, 30, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4780) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 30, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4790), new DateTime(2026, 4, 3, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4790) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 2, 19, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4800), new DateTime(2026, 4, 5, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4800) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 2, 9, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4810), new DateTime(2026, 4, 7, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4810) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 2, 24, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4820), new DateTime(2026, 4, 25, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4820) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 13, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4830), new DateTime(2026, 4, 20, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4830) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 2, 14, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4830), new DateTime(2026, 4, 28, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4830) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 1, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4870), new DateTime(2027, 9, 30, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4840) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 6, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4880), new DateTime(2028, 3, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4880) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 11, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4890), new DateTime(2027, 11, 30, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4890) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 16, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4940), new DateTime(2028, 1, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4930) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 21, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4940), new DateTime(2027, 10, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4940) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 23, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4950), new DateTime(2027, 12, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4950) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 26, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4960), new DateTime(2028, 2, 29, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 3, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4970), new DateTime(2027, 1, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4970) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 9, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4980), new DateTime(2027, 5, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4970) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 17, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4990), new DateTime(2027, 9, 30, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(4980) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 14, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5000), new DateTime(2028, 9, 30, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5000) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 21,
                column: "DateAdded",
                value: new DateTime(2026, 3, 20, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5000));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 22,
                column: "DateAdded",
                value: new DateTime(2026, 3, 15, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5010));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 23,
                column: "DateAdded",
                value: new DateTime(2026, 3, 22, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5020));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 24,
                column: "DateAdded",
                value: new DateTime(2026, 3, 10, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5020));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 25,
                column: "DateAdded",
                value: new DateTime(2026, 3, 18, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5030));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 26,
                column: "DateAdded",
                value: new DateTime(2026, 3, 25, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5040));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 27,
                column: "DateAdded",
                value: new DateTime(2026, 3, 7, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5040));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 28,
                column: "DateAdded",
                value: new DateTime(2026, 3, 24, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5050));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 29,
                column: "DateAdded",
                value: new DateTime(2026, 3, 27, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5050));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 30,
                column: "DateAdded",
                value: new DateTime(2026, 3, 4, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5060));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 31,
                column: "DateAdded",
                value: new DateTime(2026, 3, 5, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5060));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 32,
                column: "DateAdded",
                value: new DateTime(2026, 3, 16, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5070));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 33,
                column: "DateAdded",
                value: new DateTime(2026, 3, 19, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5070));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 34,
                column: "DateAdded",
                value: new DateTime(2026, 3, 2, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5080));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 35,
                column: "DateAdded",
                value: new DateTime(2026, 2, 28, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5080));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 36,
                column: "DateAdded",
                value: new DateTime(2026, 2, 26, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5090));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 19, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5100), new DateTime(2027, 6, 30, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5090) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 12, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5100), new DateTime(2028, 3, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5100) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 8, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5110), new DateTime(2028, 5, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5110) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 3, 23, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5120), new DateTime(2029, 3, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5120) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 41,
                column: "DateAdded",
                value: new DateTime(2026, 2, 17, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5130));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 2, 4, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5140), new DateTime(2026, 5, 15, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5130) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 2, 11, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5150), new DateTime(2026, 5, 20, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5140) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 44,
                column: "DateAdded",
                value: new DateTime(2026, 2, 21, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5150));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 45,
                column: "DateAdded",
                value: new DateTime(2026, 2, 7, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5160));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 46,
                column: "DateAdded",
                value: new DateTime(2026, 2, 15, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5170));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 25, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5180), new DateTime(2027, 7, 31, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5170) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 48,
                column: "DateAdded",
                value: new DateTime(2026, 1, 19, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5180));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 49,
                column: "DateAdded",
                value: new DateTime(2026, 1, 2, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5190));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 50,
                column: "DateAdded",
                value: new DateTime(2026, 1, 13, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5200));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 51,
                column: "DateAdded",
                value: new DateTime(2025, 12, 26, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5200));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 52,
                column: "DateAdded",
                value: new DateTime(2025, 12, 19, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5210));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 53,
                column: "DateAdded",
                value: new DateTime(2026, 2, 1, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5210));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 54,
                column: "DateAdded",
                value: new DateTime(2026, 1, 7, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5220));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 55,
                column: "DateAdded",
                value: new DateTime(2026, 1, 21, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5230));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 56,
                column: "DateAdded",
                value: new DateTime(2026, 2, 13, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5230));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 57,
                column: "DateAdded",
                value: new DateTime(2025, 12, 11, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5230));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 58,
                column: "DateAdded",
                value: new DateTime(2025, 12, 23, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5240));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 59,
                column: "DateAdded",
                value: new DateTime(2026, 1, 28, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5240));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "DateAdded", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 15, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5260), new DateTime(2026, 4, 2, 5, 41, 52, 378, DateTimeKind.Local).AddTicks(5250) });

            migrationBuilder.CreateIndex(
                name: "IX_DonationLogs_InventoryItemId",
                table: "DonationLogs",
                column: "InventoryItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonationLogs");

            migrationBuilder.AddColumn<string>(
                name: "DonorEmail",
                table: "InventoryItems",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DonorName",
                table: "InventoryItems",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DonorPhone",
                table: "InventoryItems",
                type: "TEXT",
                maxLength: 15,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QrCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InventoryItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Format = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "QR_CODE"),
                    GeneratedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Value = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QrCodes_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AdjustmentLogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdjustmentDate",
                value: new DateTime(2026, 1, 15, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(8321));

            migrationBuilder.UpdateData(
                table: "AdjustmentLogs",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdjustmentDate",
                value: new DateTime(2026, 1, 18, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(8523));

            migrationBuilder.UpdateData(
                table: "AdjustmentLogs",
                keyColumn: "Id",
                keyValue: 3,
                column: "AdjustmentDate",
                value: new DateTime(2026, 1, 17, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(8526));

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 9, 22, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(3644), "sarah.j@email.com", "Sarah Johnson", "555-0101", new DateTime(2025, 12, 21, 9, 6, 22, 270, DateTimeKind.Local).AddTicks(9486) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 10, 22, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4261), "m.chen@email.com", "Michael Chen", "555-0102", new DateTime(2026, 1, 5, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4257) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 8, 23, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4267), null, "Jennifer Martinez", "555-0103", new DateTime(2025, 11, 21, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4265) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 11, 21, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4271), "r.williams@email.com", "Robert Williams", null, new DateTime(2026, 1, 23, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4269) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 11, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4292), "lisa.a@email.com", "Lisa Anderson", "555-0105", new DateTime(2026, 1, 25, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4290) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 1, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4295), null, "Target Store #1234", null, new DateTime(2026, 1, 27, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4294) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 16, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4298), null, "David Thompson", "555-0107", new DateTime(2026, 2, 14, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4297) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 2, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4301), "emily.d@email.com", "Emily Davis", "555-0108", new DateTime(2026, 2, 9, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4300) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 6, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4304), "j.wilson@email.com", "James Wilson", null, new DateTime(2026, 2, 17, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4303) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 21, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4322), "patricia.m@email.com", "Patricia Moore", "555-0110", new DateTime(2027, 7, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4305) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 26, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4326), null, "Walmart Store #5678", "555-0111", new DateTime(2028, 1, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4324) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 31, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4329), "chris.t@email.com", "Christopher Taylor", null, new DateTime(2027, 9, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4328) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 5, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4340), "jessica.b@email.com", "Jessica Brown", "555-0113", new DateTime(2027, 11, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4331) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 10, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4343), null, "Daniel Garcia", "555-0114", new DateTime(2027, 8, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4342) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 12, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4346), null, "St. Mary's Church", null, new DateTime(2027, 10, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4345) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 15, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4348), "matt.r@email.com", "Matthew Rodriguez", "555-0116", new DateTime(2027, 12, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4347) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 23, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4351), "ashley.l@email.com", "Ashley Lewis", null, new DateTime(2026, 11, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4350) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 29, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4354), null, "Joshua Walker", "555-0118", new DateTime(2027, 3, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4353) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 6, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4357), "amanda.h@email.com", "Amanda Hall", "555-0119", new DateTime(2027, 7, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4356) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 3, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4359), null, "Community Health Center", null, new DateTime(2028, 7, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4358) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2026, 1, 9, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4361), null, "Ryan Allen", "555-0121" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2026, 1, 4, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4363), "samantha.y@email.com", "Samantha Young", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2026, 1, 11, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4373), "brandon.k@email.com", "Brandon King", "555-0123" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 30, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4374), null, "Nicole Wright", "555-0124" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2026, 1, 7, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4376), "justin.s@email.com", "Justin Scott", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2026, 1, 14, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4378), "megan.g@email.com", "Megan Green", "555-0126" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 27, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4380), null, "Tyler Adams", "555-0127" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2026, 1, 13, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4382), null, "Rachel Baker", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2026, 1, 16, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4383), "kevin.n@email.com", "Kevin Nelson", "555-0129" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 24, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4385), "lauren.c@email.com", "Lauren Carter", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 25, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4387), null, "Jacob Mitchell", "555-0131" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2026, 1, 5, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4389), "kim.p@email.com", "Kimberly Perez", "555-0132" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2026, 1, 8, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4391), null, "Austin Roberts", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 22, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4399), "brittany.t@email.com", "Brittany Turner", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4401), null, "Costco Wholesale", "555-0135" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 18, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4403), "zach.p@email.com", "Zachary Phillips", "555-0136" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 8, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4406), null, "Hannah Campbell", null, new DateTime(2027, 4, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4405) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 1, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4409), null, "Nathan Parker", "555-0138", new DateTime(2028, 1, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4408) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 28, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4412), "alexis.e@email.com", "Alexis Evans", null, new DateTime(2028, 3, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4411) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 12, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4415), "sam.e@email.com", "Samuel Edwards", "555-0140", new DateTime(2029, 1, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4414) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 9, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4417), null, "Grace Collins", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 11, 26, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4419), null, "Benjamin Stewart", "555-0142", new DateTime(2026, 3, 6, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4418) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4422), "victoria.s@email.com", "Victoria Sanchez", null, new DateTime(2026, 3, 11, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4421) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 13, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4431), "alex.m@email.com", "Alexander Morris", "555-0144" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 11, 29, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4441), null, "Sophia Rogers", "555-0145" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 7, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4442), null, "Elijah Reed", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 11, 16, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4445), "olivia.c@email.com", "Olivia Cook", "555-0147", new DateTime(2027, 5, 20, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4444) });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 11, 10, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4447), "mason.m@email.com", "Mason Morgan", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 10, 24, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4449), null, "Ava Bell", "555-0149" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 11, 4, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4450), "lucas.m@email.com", "Lucas Murphy", "555-0150" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 10, 17, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4452), null, "Isabella Bailey", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 10, 10, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4454), null, "Ethan Rivera", "555-0152" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 11, 23, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4456), "mia.c@email.com", "Mia Cooper", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 10, 29, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4458), "logan.r@email.com", "Logan Richardson", "555-0154" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 11, 12, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4459), null, "Charlotte Cox", "555-0155" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 12, 5, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4462), null, "Aiden Howard", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 10, 2, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4464), "amelia.w@email.com", "Amelia Ward", "555-0157" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 10, 14, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4465), "liam.t@email.com", "Liam Torres", null });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone" },
                values: new object[] { new DateTime(2025, 11, 19, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4467), null, "Harper Peterson", "555-0159" });

            migrationBuilder.UpdateData(
                table: "InventoryItems",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "DateAdded", "DonorEmail", "DonorName", "DonorPhone", "ExpirationDate" },
                values: new object[] { new DateTime(2025, 11, 6, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4470), "noah.g@email.com", "Noah Gray", "555-0160", new DateTime(2026, 1, 22, 9, 6, 22, 272, DateTimeKind.Local).AddTicks(4469) });

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_InventoryItemId",
                table: "QrCodes",
                column: "InventoryItemId");
        }
    }
}
