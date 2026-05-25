using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCostCenterAndDimensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "FinancialStatement",
                table: "LedgerAccount",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresCostCenter",
                table: "LedgerAccount",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresTercero",
                table: "LedgerAccount",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                table: "JournalEntryLine",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "JournalEntryLine",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "JournalEntryLine",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CostCenter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenter", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLine_CostCenterId",
                table: "JournalEntryLine",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLine_CustomerId",
                table: "JournalEntryLine",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLine_ProviderId",
                table: "JournalEntryLine",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_Code",
                table: "CostCenter",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLine_CostCenter_CostCenterId",
                table: "JournalEntryLine",
                column: "CostCenterId",
                principalTable: "CostCenter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLine_Customer_CustomerId",
                table: "JournalEntryLine",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLine_Provider_ProviderId",
                table: "JournalEntryLine",
                column: "ProviderId",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLine_CostCenter_CostCenterId",
                table: "JournalEntryLine");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLine_Customer_CustomerId",
                table: "JournalEntryLine");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLine_Provider_ProviderId",
                table: "JournalEntryLine");

            migrationBuilder.DropTable(
                name: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLine_CostCenterId",
                table: "JournalEntryLine");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLine_CustomerId",
                table: "JournalEntryLine");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLine_ProviderId",
                table: "JournalEntryLine");

            migrationBuilder.DropColumn(
                name: "FinancialStatement",
                table: "LedgerAccount");

            migrationBuilder.DropColumn(
                name: "RequiresCostCenter",
                table: "LedgerAccount");

            migrationBuilder.DropColumn(
                name: "RequiresTercero",
                table: "LedgerAccount");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "JournalEntryLine");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "JournalEntryLine");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "JournalEntryLine");

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
    }
}
