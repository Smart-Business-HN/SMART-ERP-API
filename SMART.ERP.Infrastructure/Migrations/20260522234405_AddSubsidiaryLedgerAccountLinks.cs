using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubsidiaryLedgerAccountLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LedgerAccountId",
                table: "Provider",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LedgerAccountId",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RevenueLedgerAccountId",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provider_LedgerAccountId",
                table: "Provider",
                column: "LedgerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_LedgerAccountId",
                table: "Customer",
                column: "LedgerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_RevenueLedgerAccountId",
                table: "Customer",
                column: "RevenueLedgerAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_LedgerAccount_LedgerAccountId",
                table: "Customer",
                column: "LedgerAccountId",
                principalTable: "LedgerAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_LedgerAccount_RevenueLedgerAccountId",
                table: "Customer",
                column: "RevenueLedgerAccountId",
                principalTable: "LedgerAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Provider_LedgerAccount_LedgerAccountId",
                table: "Provider",
                column: "LedgerAccountId",
                principalTable: "LedgerAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_LedgerAccount_LedgerAccountId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_LedgerAccount_RevenueLedgerAccountId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Provider_LedgerAccount_LedgerAccountId",
                table: "Provider");

            migrationBuilder.DropIndex(
                name: "IX_Provider_LedgerAccountId",
                table: "Provider");

            migrationBuilder.DropIndex(
                name: "IX_Customer_LedgerAccountId",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_RevenueLedgerAccountId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "LedgerAccountId",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "LedgerAccountId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "RevenueLedgerAccountId",
                table: "Customer");
        }
    }
}
