using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDropshippingSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVirtual",
                table: "Warehouse",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseTypeId",
                table: "Warehouse",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalWithoutShipping",
                table: "Quotation",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalShippingCost",
                table: "Quotation",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultShippingCost",
                table: "Provider",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultShippingDays",
                table: "Provider",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultShippingType",
                table: "Provider",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupportsDropshipping",
                table: "Provider",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFromVirtualStock",
                table: "ProductOffered",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCost",
                table: "ProductOffered",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SourceWarehouseId",
                table: "ProductOffered",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotalWithoutShipping",
                table: "ProductOffered",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ProviderWarehouse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderWarehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderWarehouse_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderWarehouse_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingCostConfiguration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceWarehouseId = table.Column<int>(type: "int", nullable: true),
                    SourceProviderId = table.Column<int>(type: "int", nullable: true),
                    SourceCityId = table.Column<int>(type: "int", nullable: true),
                    DestinationCityId = table.Column<int>(type: "int", nullable: true),
                    DestinationDepartmentId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    MinCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DefaultCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CostType = table.Column<string>(type: "varchar(50)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCostConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingCostConfiguration_City_DestinationCityId",
                        column: x => x.DestinationCityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingCostConfiguration_City_SourceCityId",
                        column: x => x.SourceCityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingCostConfiguration_Department_DestinationDepartmentId",
                        column: x => x.DestinationDepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingCostConfiguration_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingCostConfiguration_Provider_SourceProviderId",
                        column: x => x.SourceProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingCostConfiguration_Warehouse_SourceWarehouseId",
                        column: x => x.SourceWarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VirtualStockImport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "varchar(100)", nullable: false),
                    ImportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalProducts = table.Column<int>(type: "int", nullable: false),
                    SuccessfulImports = table.Column<int>(type: "int", nullable: false),
                    FailedImports = table.Column<int>(type: "int", nullable: false),
                    ErrorLog = table.Column<string>(type: "varchar(max)", nullable: true),
                    ImportedBy = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualStockImport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualStockImport_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VirtualStockImport_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(200)", nullable: true),
                    IsVirtual = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseType", x => x.Id);
                });

            // Seed WarehouseType data
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT [WarehouseType] ON;
                INSERT INTO [WarehouseType] (Id, Name, Description, IsVirtual, IsActive)
                VALUES
                    (1, 'Físico', 'Almacén físico de Smart Business', 0, 1),
                    (2, 'Virtual', 'Almacén virtual dropshipping', 1, 1);
                SET IDENTITY_INSERT [WarehouseType] OFF;
            ");

            // Update existing warehouses to Físico type
            migrationBuilder.Sql(@"
                UPDATE [Warehouse]
                SET WarehouseTypeId = 1, IsVirtual = 0
                WHERE WarehouseTypeId IS NULL;
            ");

            migrationBuilder.CreateTable(
                name: "VirtualStockImportDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VirtualStockImportId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductCode = table.Column<string>(type: "varchar(50)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    WasSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "varchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualStockImportDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualStockImportDetail_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VirtualStockImportDetail_VirtualStockImport_VirtualStockImportId",
                        column: x => x.VirtualStockImportId,
                        principalTable: "VirtualStockImport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_WarehouseTypeId",
                table: "Warehouse",
                column: "WarehouseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOffered_SourceWarehouseId",
                table: "ProductOffered",
                column: "SourceWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderWarehouse_ProviderId",
                table: "ProviderWarehouse",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderWarehouse_WarehouseId",
                table: "ProviderWarehouse",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCostConfiguration_DestinationCityId",
                table: "ShippingCostConfiguration",
                column: "DestinationCityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCostConfiguration_DestinationDepartmentId",
                table: "ShippingCostConfiguration",
                column: "DestinationDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCostConfiguration_ProductId",
                table: "ShippingCostConfiguration",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCostConfiguration_SourceCityId",
                table: "ShippingCostConfiguration",
                column: "SourceCityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCostConfiguration_SourceProviderId",
                table: "ShippingCostConfiguration",
                column: "SourceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCostConfiguration_SourceWarehouseId",
                table: "ShippingCostConfiguration",
                column: "SourceWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualStockImport_ProviderId",
                table: "VirtualStockImport",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualStockImport_WarehouseId",
                table: "VirtualStockImport",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualStockImportDetail_ProductId",
                table: "VirtualStockImportDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualStockImportDetail_VirtualStockImportId",
                table: "VirtualStockImportDetail",
                column: "VirtualStockImportId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOffered_Warehouse_SourceWarehouseId",
                table: "ProductOffered",
                column: "SourceWarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_WarehouseType_WarehouseTypeId",
                table: "Warehouse",
                column: "WarehouseTypeId",
                principalTable: "WarehouseType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOffered_Warehouse_SourceWarehouseId",
                table: "ProductOffered");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_WarehouseType_WarehouseTypeId",
                table: "Warehouse");

            migrationBuilder.DropTable(
                name: "ProviderWarehouse");

            migrationBuilder.DropTable(
                name: "ShippingCostConfiguration");

            migrationBuilder.DropTable(
                name: "VirtualStockImportDetail");

            migrationBuilder.DropTable(
                name: "WarehouseType");

            migrationBuilder.DropTable(
                name: "VirtualStockImport");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_WarehouseTypeId",
                table: "Warehouse");

            migrationBuilder.DropIndex(
                name: "IX_ProductOffered_SourceWarehouseId",
                table: "ProductOffered");

            migrationBuilder.DropColumn(
                name: "IsVirtual",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "WarehouseTypeId",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "SubTotalWithoutShipping",
                table: "Quotation");

            migrationBuilder.DropColumn(
                name: "TotalShippingCost",
                table: "Quotation");

            migrationBuilder.DropColumn(
                name: "DefaultShippingCost",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "DefaultShippingDays",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "DefaultShippingType",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "SupportsDropshipping",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "IsFromVirtualStock",
                table: "ProductOffered");

            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "ProductOffered");

            migrationBuilder.DropColumn(
                name: "SourceWarehouseId",
                table: "ProductOffered");

            migrationBuilder.DropColumn(
                name: "SubTotalWithoutShipping",
                table: "ProductOffered");
        }
    }
}
