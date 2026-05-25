namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Naturaleza de la cuenta contable (Tipo). Corresponde al primer dígito del código
    /// del catálogo hondureño: 1 Activo ... 7 Cuentas de Orden.
    /// </summary>
    public enum AccountType
    {
        Activo = 1,
        Pasivo = 2,
        Patrimonio = 3,
        Ingresos = 4,
        Costos = 5,
        Gastos = 6,
        CuentasDeOrden = 7
    }
}
