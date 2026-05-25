using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeferredRevenueAndWithholding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "WithholdingAmount",
                table: "PurchaseBill",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WithholdingBase",
                table: "PurchaseBill",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "WithholdingType",
                table: "PurchaseBill",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "WithholdingAmount",
                table: "NonBillableExpense",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WithholdingBase",
                table: "NonBillableExpense",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "WithholdingType",
                table: "NonBillableExpense",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeferredRevenue",
                table: "Invoice",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MonthsRecognized",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRecognitionDate",
                table: "Invoice",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecognitionMonths",
                table: "Invoice",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecognitionStartDate",
                table: "Invoice",
                type: "datetime2",
                nullable: true);

            // Seed de mapeos para las nuevas cuentas de sistema (idempotente vía NOT EXISTS por Key).
            // 15 = DeferredRevenueSaaS (2108001), 16 = SaaSRevenue (4103001),
            // 17 = WithholdingISR12_5 (2104001), 18 = WithholdingISR1 (2104002), 19 = WithholdingISV15 (2104003).
            var mappings = new (int Key, string Code)[]
            {
                (15, "2108001"),
                (16, "4103001"),
                (17, "2104001"),
                (18, "2104002"),
                (19, "2104003"),
            };
            foreach (var (key, code) in mappings)
            {
                migrationBuilder.Sql(
                    "INSERT INTO AccountingMapping ([Key], LedgerAccountId) " +
                    $"SELECT {key}, (SELECT Id FROM LedgerAccount WHERE Code = N'{code}') " +
                    $"WHERE NOT EXISTS (SELECT 1 FROM AccountingMapping WHERE [Key] = {key});"
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AccountingMapping WHERE [Key] IN (15, 16, 17, 18, 19);");

            migrationBuilder.DropColumn(
                name: "WithholdingAmount",
                table: "PurchaseBill");

            migrationBuilder.DropColumn(
                name: "WithholdingBase",
                table: "PurchaseBill");

            migrationBuilder.DropColumn(
                name: "WithholdingType",
                table: "PurchaseBill");

            migrationBuilder.DropColumn(
                name: "WithholdingAmount",
                table: "NonBillableExpense");

            migrationBuilder.DropColumn(
                name: "WithholdingBase",
                table: "NonBillableExpense");

            migrationBuilder.DropColumn(
                name: "WithholdingType",
                table: "NonBillableExpense");

            migrationBuilder.DropColumn(
                name: "IsDeferredRevenue",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "MonthsRecognized",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "NextRecognitionDate",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "RecognitionMonths",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "RecognitionStartDate",
                table: "Invoice");
        }
    }
}
