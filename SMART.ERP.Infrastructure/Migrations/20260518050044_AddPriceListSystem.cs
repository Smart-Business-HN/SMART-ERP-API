using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceListSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriceListId",
                table: "CustomerType",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriceListId",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PriceList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceListItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceListId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceListItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceListItem_PriceList_PriceListId",
                        column: x => x.PriceListId,
                        principalTable: "PriceList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceListItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerType_PriceListId",
                table: "CustomerType",
                column: "PriceListId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_PriceListId",
                table: "Customer",
                column: "PriceListId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceList_IsDefault",
                table: "PriceList",
                column: "IsDefault",
                unique: true,
                filter: "[IsDefault] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_PriceList_Name",
                table: "PriceList",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItem_PriceListId_ProductId",
                table: "PriceListItem",
                columns: new[] { "PriceListId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItem_ProductId",
                table: "PriceListItem",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_PriceList_PriceListId",
                table: "Customer",
                column: "PriceListId",
                principalTable: "PriceList",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerType_PriceList_PriceListId",
                table: "CustomerType",
                column: "PriceListId",
                principalTable: "PriceList",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Seed: reproduce los precios actuales del switch hardcoded de ProductPricingService.
            // Idempotente: solo se ejecuta si la tabla PriceList está vacía.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM PriceList)
BEGIN
    -- Lista pública (default) — reemplaza el margen anónimo de 30%
    INSERT INTO PriceList (Name, Description, IsDefault, IsActive, CreationDate, CreatedBy)
    VALUES (N'Lista Público', N'Lista por defecto para visitantes anónimos y fallback general', 1, 1, GETUTCDATE(), N'system-migration');

    -- Una lista por cada CustomerType activo
    INSERT INTO PriceList (Name, Description, IsDefault, IsActive, CreationDate, CreatedBy)
    SELECT N'Lista ' + ct.Name,
           N'Auto-generada desde CustomerType ' + ct.Name + N' durante migración AddPriceListSystem',
           0, 1, GETUTCDATE(), N'system-migration'
    FROM CustomerType ct
    WHERE ct.IsActive = 1
      AND NOT EXISTS (SELECT 1 FROM PriceList pl WHERE pl.Name = N'Lista ' + ct.Name);

    -- Asignar PriceListId al CustomerType correspondiente
    UPDATE ct
    SET ct.PriceListId = pl.Id
    FROM CustomerType ct
    INNER JOIN PriceList pl ON pl.Name = N'Lista ' + ct.Name
    WHERE ct.PriceListId IS NULL;

    -- Seed PriceListItem en la lista pública (margen 1.30, replica el anónimo)
    INSERT INTO PriceListItem (PriceListId, ProductId, Price, CreationDate, CreatedBy)
    SELECT pl.Id, p.Id,
           CEILING((p.CostPrice * 1.30) * (1 + ISNULL(t.Rate, 0) / 100.0)),
           GETUTCDATE(), N'system-migration'
    FROM Product p
    LEFT JOIN Tax t ON t.Id = p.TaxId
    CROSS APPLY (SELECT TOP 1 Id FROM PriceList WHERE IsDefault = 1) pl
    WHERE p.IsActive = 1 AND p.CostPrice > 0
      AND NOT EXISTS (SELECT 1 FROM PriceListItem pli WHERE pli.PriceListId = pl.Id AND pli.ProductId = p.Id);

    -- Seed PriceListItem por CustomerType usando márgenes hardcoded actuales
    -- Margins: Basico(1)=1.25, Recurrente(2)=1.18, Mayorista(3)=1.08, Integrador(4)=1.10,
    --          Corporativo(5)=1.10, Empleado(6)=1.05, otros=1.20 (default del switch)
    ;WITH Margins AS (
        SELECT 1 AS TypeId, CAST(1.25 AS DECIMAL(18,4)) AS Mult UNION ALL
        SELECT 2, 1.18 UNION ALL
        SELECT 3, 1.08 UNION ALL
        SELECT 4, 1.10 UNION ALL
        SELECT 5, 1.10 UNION ALL
        SELECT 6, 1.05
    )
    INSERT INTO PriceListItem (PriceListId, ProductId, Price, CreationDate, CreatedBy)
    SELECT ct.PriceListId, p.Id,
           CEILING((p.CostPrice * ISNULL(m.Mult, 1.20)) * (1 + ISNULL(t.Rate, 0) / 100.0)),
           GETUTCDATE(), N'system-migration'
    FROM Product p
    LEFT JOIN Tax t ON t.Id = p.TaxId
    CROSS JOIN CustomerType ct
    LEFT JOIN Margins m ON m.TypeId = ct.Id
    WHERE p.IsActive = 1
      AND p.CostPrice > 0
      AND ct.PriceListId IS NOT NULL
      AND NOT EXISTS (SELECT 1 FROM PriceListItem pli WHERE pli.PriceListId = ct.PriceListId AND pli.ProductId = p.Id);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_PriceList_PriceListId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerType_PriceList_PriceListId",
                table: "CustomerType");

            migrationBuilder.DropTable(
                name: "PriceListItem");

            migrationBuilder.DropTable(
                name: "PriceList");

            migrationBuilder.DropIndex(
                name: "IX_CustomerType_PriceListId",
                table: "CustomerType");

            migrationBuilder.DropIndex(
                name: "IX_Customer_PriceListId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PriceListId",
                table: "CustomerType");

            migrationBuilder.DropColumn(
                name: "PriceListId",
                table: "Customer");
        }
    }
}
