using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDescriptionToCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductDescription",
                table: "CartItem",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductDescription",
                table: "CartItem");
        }
    }
}
