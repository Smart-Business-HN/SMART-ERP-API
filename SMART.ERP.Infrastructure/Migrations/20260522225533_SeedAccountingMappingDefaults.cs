using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <summary>
    /// Siembra la configuración del posteo automático: el toggle (apagado) y las 14 cuentas de
    /// sistema mapeadas a códigos del catálogo hondureño. Las cuentas no imputables (p.ej. 1102002,
    /// 4101001, 2101001 tienen auxiliares) deberán ajustarse en "Configuración Contable" antes de
    /// activar el posteo; el toggle arranca apagado para no romper operaciones.
    /// </summary>
    public partial class SeedAccountingMappingDefaults : Migration
    {
        // AccountMappingKey (int) -> código de cuenta del catálogo.
        private static readonly (int Key, string Code)[] Defaults =
        {
            (1,  "1102002"),      // AccountsReceivable
            (2,  "4101001"),      // SalesRevenue
            (3,  "21010050001"),  // SalesTaxPayable15
            (4,  "21010050003"),  // SalesTaxPayable18
            (5,  "5101001"),      // CostOfGoodsSold
            (6,  "1108001"),      // Inventory
            (7,  "2101001"),      // AccountsPayable
            (8,  "1302001"),      // InputTaxCredit15
            (9,  "1302001"),      // InputTaxCredit18 (mismo crédito fiscal ISV)
            (10, "1101001"),      // CashOnHand
            (11, "1108006"),      // InventoryAdjustmentIncrease
            (12, "1108009"),      // InventoryAdjustmentDecrease
            (13, "3101001"),      // OpeningEquity
            (14, "6104001"),      // DefaultExpense
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM AccountingSettings) INSERT INTO AccountingSettings (AutoPostingEnabled) VALUES (0);");

            foreach (var (key, code) in Defaults)
            {
                migrationBuilder.Sql(
                    "INSERT INTO AccountingMapping ([Key], LedgerAccountId) " +
                    $"SELECT {key}, (SELECT Id FROM LedgerAccount WHERE Code = N'{code}') " +
                    $"WHERE NOT EXISTS (SELECT 1 FROM AccountingMapping WHERE [Key] = {key});");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AccountingMapping;");
            migrationBuilder.Sql("DELETE FROM AccountingSettings;");
        }
    }
}
