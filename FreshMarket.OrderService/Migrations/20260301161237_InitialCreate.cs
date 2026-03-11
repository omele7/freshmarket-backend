using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshMarket.OrderService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Identificador único del item en el carrito")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false, comment: "ID del usuario (referencia a UserService)"),
                    ProductId = table.Column<int>(type: "int", nullable: false, comment: "ID del producto (referencia a ProductService)"),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Cantidad de unidades del producto"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Fecha de creación del item"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Fecha de última actualización")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.CheckConstraint("CK_CartItems_Quantity_Positive", "[Quantity] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Identificador único del pedido (auto-incremental)")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "Número de orden que agrupa todos los productos de un mismo checkout"),
                    UserId = table.Column<int>(type: "int", nullable: false, comment: "Identificador del usuario que realiza el pedido (int, referencia a UserService)"),
                    ProductId = table.Column<int>(type: "int", nullable: false, comment: "Identificador del producto en el pedido (int, referencia a ProductService)"),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Cantidad de productos en el pedido"),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "Precio unitario del producto al momento de la compra"),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "Precio total del pedido (Quantity * UnitPrice)"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Fecha y hora de creación del pedido (UTC)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.CheckConstraint("CK_Orders_Quantity_Positive", "[Quantity] > 0");
                    table.CheckConstraint("CK_Orders_TotalPrice_NonNegative", "[TotalPrice] >= 0");
                    table.CheckConstraint("CK_Orders_UnitPrice_NonNegative", "[UnitPrice] >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId_ProductId",
                table: "CartItems",
                columns: new[] { "UserId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedAt",
                table: "Orders",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId_CreatedAt",
                table: "Orders",
                columns: new[] { "ProductId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId_CreatedAt",
                table: "Orders",
                columns: new[] { "UserId", "CreatedAt" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
