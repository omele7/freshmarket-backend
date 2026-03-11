using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FreshMarket.ProductService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsAvailable", "Name", "Price", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Frutas", new DateTime(2026, 2, 18, 6, 48, 7, 143, DateTimeKind.Utc).AddTicks(1764), "Manzanas rojas frescas de la mejor calidad", "https://example.com/images/manzana-roja.jpg", true, "Manzana Roja", 2.50m, 100, null },
                    { 2, "Verduras", new DateTime(2026, 2, 18, 6, 48, 7, 143, DateTimeKind.Utc).AddTicks(1767), "Lechuga fresca cultivada orgánicamente", "https://example.com/images/lechuga.jpg", true, "Lechuga Orgánica", 1.80m, 50, null },
                    { 3, "Lácteos", new DateTime(2026, 2, 18, 6, 48, 7, 143, DateTimeKind.Utc).AddTicks(1768), "Leche entera pasteurizada 1L", "https://example.com/images/leche.jpg", true, "Leche Entera", 3.20m, 75, null },
                    { 4, "Panadería", new DateTime(2026, 2, 18, 6, 48, 7, 143, DateTimeKind.Utc).AddTicks(1770), "Pan integral recién horneado", "https://example.com/images/pan-integral.jpg", true, "Pan Integral", 2.00m, 30, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsAvailable",
                table: "Products",
                column: "IsAvailable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
