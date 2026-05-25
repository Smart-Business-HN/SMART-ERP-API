using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryModuleVentix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InventoryDistribution",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "InventoryDistribution",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxStock",
                table: "InventoryDistribution",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinStock",
                table: "InventoryDistribution",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "InventoryDistribution",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "InventoryDistribution",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InventoryEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    PrefixId = table.Column<int>(type: "int", nullable: false),
                    EntryType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    ProviderId = table.Column<int>(type: "int", nullable: true),
                    SupplierReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ConfirmedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryEntry_Prefix_PrefixId",
                        column: x => x.PrefixId,
                        principalTable: "Prefix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryEntry_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryEntry_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryExit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    PrefixId = table.Column<int>(type: "int", nullable: false),
                    ExitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExitReason = table.Column<int>(type: "int", nullable: false),
                    CustomReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BeneficiaryName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ConfirmedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryExit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryExit_Prefix_PrefixId",
                        column: x => x.PrefixId,
                        principalTable: "Prefix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryExit_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryMovement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    MovementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MovementType = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "varchar(50)", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    DocumentCode = table.Column<string>(type: "varchar(50)", nullable: true),
                    ThirdPartyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    QuantityIn = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    QuantityOut = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RunningQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    RunningAverageCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    RunningTotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCancellation = table.Column<bool>(type: "bit", nullable: false),
                    CancelledMovementId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryMovement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryMovement_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryMovement_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseTransfer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    PrefixId = table.Column<int>(type: "int", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OriginWarehouseId = table.Column<int>(type: "int", nullable: false),
                    DestinationWarehouseId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseTransfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseTransfer_Prefix_PrefixId",
                        column: x => x.PrefixId,
                        principalTable: "Prefix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseTransfer_Warehouse_DestinationWarehouseId",
                        column: x => x.DestinationWarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseTransfer_Warehouse_OriginWarehouseId",
                        column: x => x.OriginWarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryEntryItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryEntryId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PreviousCostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PreviousStock = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryEntryItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryEntryItem_InventoryEntry_InventoryEntryId",
                        column: x => x.InventoryEntryId,
                        principalTable: "InventoryEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryEntryItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryExitItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryExitId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PreviousStock = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryExitItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryExitItem_InventoryExit_InventoryExitId",
                        column: x => x.InventoryExitId,
                        principalTable: "InventoryExit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryExitItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseTransferItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseTransferId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseTransferItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseTransferItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseTransferItem_WarehouseTransfer_WarehouseTransferId",
                        column: x => x.WarehouseTransferId,
                        principalTable: "WarehouseTransfer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntry_PrefixId",
                table: "InventoryEntry",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntry_ProviderId",
                table: "InventoryEntry",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntry_WarehouseId",
                table: "InventoryEntry",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntryItem_InventoryEntryId",
                table: "InventoryEntryItem",
                column: "InventoryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntryItem_ProductId",
                table: "InventoryEntryItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryExit_PrefixId",
                table: "InventoryExit",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryExit_WarehouseId",
                table: "InventoryExit",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryExitItem_InventoryExitId",
                table: "InventoryExitItem",
                column: "InventoryExitId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryExitItem_ProductId",
                table: "InventoryExitItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovement_DocumentType_DocumentId",
                table: "InventoryMovement",
                columns: new[] { "DocumentType", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovement_ProductId_WarehouseId_MovementDate",
                table: "InventoryMovement",
                columns: new[] { "ProductId", "WarehouseId", "MovementDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovement_WarehouseId",
                table: "InventoryMovement",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransfer_DestinationWarehouseId",
                table: "WarehouseTransfer",
                column: "DestinationWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransfer_OriginWarehouseId",
                table: "WarehouseTransfer",
                column: "OriginWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransfer_PrefixId",
                table: "WarehouseTransfer",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferItem_ProductId",
                table: "WarehouseTransferItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferItem_WarehouseTransferId",
                table: "WarehouseTransferItem",
                column: "WarehouseTransferId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryEntryItem");

            migrationBuilder.DropTable(
                name: "InventoryExitItem");

            migrationBuilder.DropTable(
                name: "InventoryMovement");

            migrationBuilder.DropTable(
                name: "WarehouseTransferItem");

            migrationBuilder.DropTable(
                name: "InventoryEntry");

            migrationBuilder.DropTable(
                name: "InventoryExit");

            migrationBuilder.DropTable(
                name: "WarehouseTransfer");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InventoryDistribution");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "InventoryDistribution");

            migrationBuilder.DropColumn(
                name: "MaxStock",
                table: "InventoryDistribution");

            migrationBuilder.DropColumn(
                name: "MinStock",
                table: "InventoryDistribution");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "InventoryDistribution");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "InventoryDistribution");
        }
    }
}
