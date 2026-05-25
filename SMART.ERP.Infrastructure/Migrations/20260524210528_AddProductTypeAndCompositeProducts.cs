using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTypeAndCompositeProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductParts_FatherProductId' AND object_id = OBJECT_ID('dbo.ProductParts'))
    DROP INDEX [IX_ProductParts_FatherProductId] ON [ProductParts];");

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "ProductParts",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_ProductParts_FatherProductId_IsActive",
                table: "ProductParts",
                columns: new[] { "FatherProductId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductParts_FatherProductId_IsActive' AND object_id = OBJECT_ID('dbo.ProductParts'))
    DROP INDEX [IX_ProductParts_FatherProductId_IsActive] ON [ProductParts];");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductParts");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "Product");

            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductParts_FatherProductId' AND object_id = OBJECT_ID('dbo.ProductParts'))
    CREATE INDEX [IX_ProductParts_FatherProductId] ON [ProductParts] ([FatherProductId]);");
        }
    }
}
