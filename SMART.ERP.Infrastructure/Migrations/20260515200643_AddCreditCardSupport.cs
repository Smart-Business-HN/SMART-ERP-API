using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCardSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LegacyMigratedToInternalBankAccountId",
                table: "NonBillableExpense",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpectedPaymentDate",
                table: "Invoice",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "InternalBankAccount",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "CardLastFour",
                table: "InternalBankAccount",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NonBillableExpense_LegacyMigratedToInternalBankAccountId",
                table: "NonBillableExpense",
                column: "LegacyMigratedToInternalBankAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_NonBillableExpense_InternalBankAccount_LegacyMigratedToInternalBankAccountId",
                table: "NonBillableExpense",
                column: "LegacyMigratedToInternalBankAccountId",
                principalTable: "InternalBankAccount",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NonBillableExpense_InternalBankAccount_LegacyMigratedToInternalBankAccountId",
                table: "NonBillableExpense");

            migrationBuilder.DropIndex(
                name: "IX_NonBillableExpense_LegacyMigratedToInternalBankAccountId",
                table: "NonBillableExpense");

            migrationBuilder.DropColumn(
                name: "LegacyMigratedToInternalBankAccountId",
                table: "NonBillableExpense");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "InternalBankAccount");

            migrationBuilder.DropColumn(
                name: "CardLastFour",
                table: "InternalBankAccount");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpectedPaymentDate",
                table: "Invoice",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
