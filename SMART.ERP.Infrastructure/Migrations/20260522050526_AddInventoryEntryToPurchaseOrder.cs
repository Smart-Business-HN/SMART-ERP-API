using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryEntryToPurchaseOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InventoryEntryDestinationId",
                table: "PurchaseOrder",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderOriginId",
                table: "InventoryEntry",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_InventoryEntryDestinationId",
                table: "PurchaseOrder",
                column: "InventoryEntryDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntry_PurchaseOrderOriginId",
                table: "InventoryEntry",
                column: "PurchaseOrderOriginId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryEntry_PurchaseOrder_PurchaseOrderOriginId",
                table: "InventoryEntry",
                column: "PurchaseOrderOriginId",
                principalTable: "PurchaseOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrder_InventoryEntry_InventoryEntryDestinationId",
                table: "PurchaseOrder",
                column: "InventoryEntryDestinationId",
                principalTable: "InventoryEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryEntry_PurchaseOrder_PurchaseOrderOriginId",
                table: "InventoryEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_InventoryEntry_InventoryEntryDestinationId",
                table: "PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrder_InventoryEntryDestinationId",
                table: "PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_InventoryEntry_PurchaseOrderOriginId",
                table: "InventoryEntry");

            migrationBuilder.DropColumn(
                name: "InventoryEntryDestinationId",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderOriginId",
                table: "InventoryEntry");
        }
    }
}
