using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSubcategoryManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductSubcategoryLink",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SubcategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSubcategoryLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSubcategoryLink_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductSubcategoryLink_Subcategory_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "Subcategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubcategoryLink_ProductId_SubcategoryId",
                table: "ProductSubcategoryLink",
                columns: new[] { "ProductId", "SubcategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubcategoryLink_SubcategoryId",
                table: "ProductSubcategoryLink",
                column: "SubcategoryId");

            // Sembrar membresías existentes: cada producto queda asociado (al menos) a su subcategoría
            // principal actual, para que ningún producto pierda su ubicación tras la migración.
            migrationBuilder.Sql(
                "INSERT INTO ProductSubcategoryLink (ProductId, SubcategoryId) SELECT Id, SubCategoryId FROM Product;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSubcategoryLink");
        }
    }
}
