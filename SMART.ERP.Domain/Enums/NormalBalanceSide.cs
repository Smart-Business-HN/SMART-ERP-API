namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Saldo normal de la cuenta. Activo/Costos/Gastos = Deudor (Debit);
    /// Pasivo/Patrimonio/Ingresos = Acreedor (Credit).
    /// </summary>
    public enum NormalBalanceSide
    {
        Debit = 1,
        Credit = 2
    }
}
