namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Estado de un período/ejercicio fiscal. En un período Closed no se permite contabilizar,
    /// editar ni eliminar asientos (Código de Comercio Arts. 430-433).
    /// </summary>
    public enum FiscalPeriodStatus
    {
        Open = 1,
        Closed = 2
    }
}
