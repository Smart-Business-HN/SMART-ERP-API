using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Helpers
{
    public static class AccountMappingKeyLabels
    {
        public static string Label(AccountMappingKey key) => key switch
        {
            AccountMappingKey.AccountsReceivable => "Cuentas por cobrar (clientes)",
            AccountMappingKey.SalesRevenue => "Ingreso por ventas",
            AccountMappingKey.SalesTaxPayable15 => "ISV 15% por pagar (débito fiscal)",
            AccountMappingKey.SalesTaxPayable18 => "ISV 18% por pagar (débito fiscal)",
            AccountMappingKey.CostOfGoodsSold => "Costo de venta",
            AccountMappingKey.Inventory => "Inventario",
            AccountMappingKey.AccountsPayable => "Cuentas por pagar (proveedores)",
            AccountMappingKey.InputTaxCredit15 => "Crédito fiscal ISV 15% (compras)",
            AccountMappingKey.InputTaxCredit18 => "Crédito fiscal ISV 18% (compras)",
            AccountMappingKey.CashOnHand => "Caja / banco por defecto",
            AccountMappingKey.InventoryAdjustmentIncrease => "Ajuste de inventario (incremento)",
            AccountMappingKey.InventoryAdjustmentDecrease => "Ajuste de inventario (disminución)",
            AccountMappingKey.OpeningEquity => "Contrapartida de inventario inicial",
            AccountMappingKey.DefaultExpense => "Gasto por defecto",
            _ => key.ToString()
        };
    }
}
