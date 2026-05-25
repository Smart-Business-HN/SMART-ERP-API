namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Claves de las cuentas de sistema usadas por el posteo automático para resolver a qué
    /// cuenta del catálogo va cada concepto. Cada clave se mapea a un LedgerAccount imputable.
    /// </summary>
    public enum AccountMappingKey
    {
        AccountsReceivable = 1,          // Cuentas por cobrar clientes
        SalesRevenue = 2,                // Ingreso por ventas
        SalesTaxPayable15 = 3,           // ISV 15% por pagar (débito fiscal)
        SalesTaxPayable18 = 4,           // ISV 18% por pagar (débito fiscal)
        CostOfGoodsSold = 5,             // Costo de venta
        Inventory = 6,                   // Inventario
        AccountsPayable = 7,             // Cuentas por pagar proveedores
        InputTaxCredit15 = 8,            // Crédito fiscal ISV 15% por compras
        InputTaxCredit18 = 9,            // Crédito fiscal ISV 18% por compras
        CashOnHand = 10,                 // Caja (banco/caja por defecto)
        InventoryAdjustmentIncrease = 11,// Ajuste de inventario por incremento
        InventoryAdjustmentDecrease = 12,// Ajuste de inventario por disminución
        OpeningEquity = 13,              // Contrapartida de inventario inicial
        DefaultExpense = 14,             // Gasto por defecto (fallback)
        DeferredRevenueSaaS = 15,        // Ingresos diferidos por suscripciones SaaS (2108001)
        SaaSRevenue = 16,                // Ingreso devengado de suscripciones SaaS (4103001)
        WithholdingISR12_5 = 17,         // Retención ISR 12.5% sobre honorarios profesionales (2104001)
        WithholdingISR1 = 18,            // Retención ISR 1% sobre bienes y servicios (2104002)
        WithholdingISV15 = 19            // Retención ISV 15% Art. 13 (2104003)
    }
}
