using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingPostingConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LedgerAccountId",
                table: "InternalBankAccount",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountingMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<int>(type: "int", nullable: false),
                    LedgerAccountId = table.Column<int>(type: "int", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingMapping_LedgerAccount_LedgerAccountId",
                        column: x => x.LedgerAccountId,
                        principalTable: "LedgerAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountingSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AutoPostingEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InternalBankAccount_LedgerAccountId",
                table: "InternalBankAccount",
                column: "LedgerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingMapping_Key",
                table: "AccountingMapping",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingMapping_LedgerAccountId",
                table: "AccountingMapping",
                column: "LedgerAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_InternalBankAccount_LedgerAccount_LedgerAccountId",
                table: "InternalBankAccount",
                column: "LedgerAccountId",
                principalTable: "LedgerAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InternalBankAccount_LedgerAccount_LedgerAccountId",
                table: "InternalBankAccount");

            migrationBuilder.DropTable(
                name: "AccountingMapping");

            migrationBuilder.DropTable(
                name: "AccountingSettings");

            migrationBuilder.DropIndex(
                name: "IX_InternalBankAccount_LedgerAccountId",
                table: "InternalBankAccount");

            migrationBuilder.DropColumn(
                name: "LedgerAccountId",
                table: "InternalBankAccount");
        }
    }
}
